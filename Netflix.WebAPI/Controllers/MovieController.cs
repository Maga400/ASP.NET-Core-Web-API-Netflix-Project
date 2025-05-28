using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Netflix.WebAPI.Services.Abstracts;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Netflix.WebAPI.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class MovieController : ControllerBase
    {
        private readonly ITmdbService _tmdb;
        public MovieController(ITmdbService tmdb)
        {
            _tmdb = tmdb;
        }

        [HttpGet("allMovies/{page}")]
        public async Task<IActionResult> GetAllMovies(int page, [FromQuery] string lang = "en-US", [FromQuery] int count = 20)
        {
            string endpoint = $"discover/movie?language={lang}&page={page}";

            var json = await _tmdb.GetFromTmdbAsync(endpoint);

            using var document = JsonDocument.Parse(json);
            var root = document.RootElement;

            var totalPages = root.GetProperty("total_pages").GetInt32();
            var totalResults = root.GetProperty("total_results").GetInt32();

            var results = root.GetProperty("results")
                              .EnumerateArray()
                              .Take(count)
                              .Select(x => JsonSerializer.Deserialize<object>(x.GetRawText()))
                              .ToList();
            return Ok(new
            {
                success = true,
                page = page,
                totalPages,
                totalResults,
                movies = results
            });
        }

        [HttpGet("trending/{page}")]
        public async Task<IActionResult> GetTrendingMovies(int page, [FromQuery] string lang = "en-US", [FromQuery] int count = 10)
        {
            string endpoint = $"trending/movie/day?language={lang}&page={page}";
            var json = await _tmdb.GetFromTmdbAsync(endpoint);

            using var document = JsonDocument.Parse(json);
            var root = document.RootElement;

            var totalPages = root.GetProperty("total_pages").GetInt32();
            var totalResults = root.GetProperty("total_results").GetInt32();

            var results = root.GetProperty("results")
                              .EnumerateArray()
                              .Take(count)
                              .Select(x => JsonSerializer.Deserialize<object>(x.GetRawText()))
                              .ToList();
            return Ok(new
            {
                success = true,
                page,
                totalPages,
                totalResults,
                movies = results
            });
        }

        [HttpGet("{id}/trailers")]
        public async Task<IActionResult> GetMovieTrailers(int id, [FromQuery] string lang = "en-US")
        {
            string endpoint = $"movie/{id}/videos?language={lang}";
            var json = await _tmdb.GetFromTmdbAsync(endpoint);

            List<object> trailerList = new();

            using (var doc = JsonDocument.Parse(json))
            {
                var trailers = doc.RootElement.GetProperty("results")
                                              .EnumerateArray();

                foreach (var trailer in trailers)
                {
                    trailerList.Add(JsonSerializer.Deserialize<object>(trailer.GetRawText()));
                }
            }

            return Ok(new
            {
                success = true,
                trailers = trailerList
            });
        }

        [HttpGet("{id}/details")]
        public async Task<IActionResult> GetMovieDetails(int id, [FromQuery] string lang = "en-US")
        {
            string endpoint = $"movie/{id}?language={lang}";
            var json = await _tmdb.GetFromTmdbAsync(endpoint);

            using var doc = JsonDocument.Parse(json);
            var clonedRoot = doc.RootElement.Clone();

            return Ok(new
            {
                success = true,
                content = clonedRoot
            });
        }

        [HttpGet("{id}/similar")]
        public async Task<IActionResult> GetSimilarMovies(int id, [FromQuery] string lang = "en-US", [FromQuery] int count = 20)
        {
            string endpoint = $"movie/{id}/similar?language={lang}&page=1";
            var json = await _tmdb.GetFromTmdbAsync(endpoint);

            List<object> similarItems = new();

            using (var doc = JsonDocument.Parse(json))
            {
                var results = doc.RootElement.GetProperty("results")
                                             .EnumerateArray()
                                             .Take(count);

                foreach (var item in results)
                {
                    similarItems.Add(JsonSerializer.Deserialize<object>(item.GetRawText()));
                }
            }

            return Ok(new
            {
                success = true,
                similar = similarItems
            });
        }

        [HttpGet("categories")]
        public async Task<IActionResult> GetMoviesByCategory([FromQuery] string category, [FromQuery] int page = 1, [FromQuery] string lang = "en-US", [FromQuery] int count = 20)
        {
            int perPage = count;
            string endpoint = $"movie/{category}?language={lang}&page={page}";

            var json = await _tmdb.GetFromTmdbAsync(endpoint);

            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            var totalPages = root.GetProperty("total_pages").GetInt32();
            var totalResults = root.GetProperty("total_results").GetInt32();

            var results = root.GetProperty("results")
                              .EnumerateArray()
                              .Take(perPage)
                              .Select(r => new
                              {
                                  id = r.GetProperty("id").GetInt32(),
                                  title = r.GetProperty("title").GetString(),
                                  overview = r.GetProperty("overview").GetString(),
                                  poster_path = r.GetProperty("poster_path").GetString(),
                                  release_date = r.GetProperty("release_date").GetString()
                              })
                              .ToList();

            return Ok(new
            {
                success = true,
                page = page,
                totalPages = totalPages,
                totalResults = totalResults,
                movies = results
            });
        }

        [HttpGet("genres")]
        public async Task<IActionResult> GetMovieGenres([FromQuery] string lang = "en-US")
        {
            string endpoint = $"genre/movie/list?language={lang}";
            var json = await _tmdb.GetFromTmdbAsync(endpoint);

            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            var genresElement = root.GetProperty("genres");

            var genresJson = genresElement.GetRawText(); // JsonElement -> string
            var genres = JsonSerializer.Deserialize<object>(genresJson); // string -> object

            return Ok(new
            {
                success = true,
                genres
            });
        }


        [HttpGet("genresByName")]
        public async Task<IActionResult> GetMoviesByGenreName([FromQuery] string genreName, [FromQuery] int? page, [FromQuery] string lang = "en-US", [FromQuery] int count = 20)
        {
            int pageNumber = page ?? 1;

            if (string.IsNullOrWhiteSpace(genreName))
            {
                return BadRequest(new { success = false, message = "Genre name is required" });
            }

            var genresJson = await _tmdb.GetFromTmdbAsync($"genre/movie/list?language={lang}");
            using var genresDoc = JsonDocument.Parse(genresJson);
            var genres = genresDoc.RootElement.GetProperty("genres").EnumerateArray();

            JsonElement? genre = null;
            foreach (var g in genres)
            {
                if (g.GetProperty("name").GetString().Equals(genreName, StringComparison.OrdinalIgnoreCase))
                {
                    genre = g;
                    break;
                }
            }

            if (genre == null)
            {
                return NotFound(new { success = false, message = $"Genre '{genreName}' not found" });
            }

            var genreId = genre.Value.GetProperty("id").GetInt32();

            var moviesJson = await _tmdb.GetFromTmdbAsync($"discover/movie?with_genres={genreId}&language={lang}&page={pageNumber}");
            using var moviesDoc = JsonDocument.Parse(moviesJson);
            var root = moviesDoc.RootElement;

            var totalPages = root.GetProperty("total_pages").GetInt32();
            var totalResults = root.GetProperty("total_results").GetInt32();

            // 🔧 Asıl çözüm burası:
            var moviesRawText = root.GetProperty("results").GetRawText();
            var moviesList = JsonSerializer.Deserialize<List<JsonElement>>(moviesRawText);

            var limitedMovies = moviesList.Take(count).ToList();

            return Ok(new
            {
                success = true,
                genre = genreName,
                page = pageNumber,
                totalPages,
                totalResults,
                movies = limitedMovies,
            });
        }

    }

}
