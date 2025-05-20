using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Netflix.WebAPI.Services.Abstracts;
using System.Text.Json;

namespace Netflix.WebAPI.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class TvController : ControllerBase
    {
        private readonly ITmdbService _tmdb;
        public TvController(ITmdbService tmdb)
        {
            _tmdb = tmdb;
        }

        [HttpGet("allTvShows/{page}")]
        public async Task<IActionResult> GetAllTvShows(int page, [FromQuery] string lang = "en-US", [FromQuery] int count = 20)
        {
            string endpoint = $"discover/tv?language={lang}&page={page}";

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
                tvShows = results
            });
        }

        [HttpGet("trending/{page}")]
        public async Task<IActionResult> GetTrendingTvShows(int page, [FromQuery] string lang = "en-US", [FromQuery] int count = 10)
        {
            string endpoint = $"trending/tv/day?language={lang}&page={page}";
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
                tvShows = results
            });
        }

        [Authorize]
        [HttpGet("{id}/trailers")]
        public async Task<IActionResult> GetTvShowTrailers(int id, [FromQuery] string lang = "en-US")
        {
            string endpoint = $"tv/{id}/videos?language={lang}";
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
        public async Task<IActionResult> GetTvShowDetails(int id, [FromQuery] string lang = "en-US")
        {
            string endpoint = $"tv/{id}?language={lang}";
            var json = await _tmdb.GetFromTmdbAsync(endpoint);

            using var doc = JsonDocument.Parse(json);
            var clonedRoot = doc.RootElement.Clone();

            return Ok(new
            {
                success = true,
                content = clonedRoot
            });
        }

        [Authorize]
        [HttpGet("{id}/similar")]
        public async Task<IActionResult> GetSimilarTvShows(int id, [FromQuery] string lang = "en-US", [FromQuery] int count = 20)
        {
            string endpoint = $"tv/{id}/similar?language={lang}&page=1";
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
        public async Task<IActionResult> GetTvShowsByCategory([FromQuery] string category, [FromQuery] int page = 1, [FromQuery] string lang = "en-US")
        {
            int perPage = 20;
            string endpoint = $"tv/{category}?language={lang}&page={page}";

            var json = await _tmdb.GetFromTmdbAsync(endpoint);

            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            var totalPages = root.GetProperty("total_pages").GetInt32();
            var totalResults = root.GetProperty("total_results").GetInt32();
            var results = root.GetProperty("results").EnumerateArray().Take(perPage).ToArray();

            return Ok(new
            {
                success = true,
                page,
                totalPages,
                totalResults,
                tvShows = results
            });
        }

        [HttpGet("genres")]
        public async Task<IActionResult> GetTvShowGenres([FromQuery] string lang = "en-US")
        {
            string endpoint = $"genre/tv/list?language={lang}";
            var json = await _tmdb.GetFromTmdbAsync(endpoint);

            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            var genres = root.GetProperty("genres");

            return Ok(new
            {
                success = true,
                genres
            });
        }

        [HttpGet("genresByName")]
        public async Task<IActionResult> GetTvShowsByGenreName([FromQuery] string genreName, [FromQuery] int? page, [FromQuery] string lang = "en-US")
        {
            const int perPage = 20;
            int pageNumber = page ?? 1;

            if (string.IsNullOrWhiteSpace(genreName))
            {
                return BadRequest(new { success = false, message = "Genre name is required" });
            }

            var genresJson = await _tmdb.GetFromTmdbAsync($"genre/tv/list?language={lang}");
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

            var tvShowsJson = await _tmdb.GetFromTmdbAsync($"discover/tv?with_genres={genreId}&language={lang}&page={pageNumber}");
            using var tvShowsDoc = JsonDocument.Parse(tvShowsJson);
            var root = tvShowsDoc.RootElement;

            var totalPages = root.GetProperty("total_pages").GetInt32();
            var totalResults = root.GetProperty("total_results").GetInt32();
            var tvShows = root.GetProperty("results").EnumerateArray().Take(perPage);

            return Ok(new
            {
                success = true,
                genre = genreName,
                page = pageNumber,
                totalPages,
                totalResults,
                tvShows
            });
        }
    }
}
