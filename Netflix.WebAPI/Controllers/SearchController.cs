using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Netflix.WebAPI.Services.Abstracts;
using Netflix.WebAPI.Services.Concretes;
using System.Text.Json;

namespace Netflix.WebAPI.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        private readonly ITmdbService _tmdb;
        public SearchController(ITmdbService tmdb)
        {
            _tmdb = tmdb;
        }

        [HttpGet("person/{query}")]
        public async Task<IActionResult> SearchPerson(string query, [FromQuery] string lang = "en-US")
        {
            var relativeUrl = $"search/person?query={Uri.EscapeDataString(query)}&include_adult=false&language={lang}&page=1";
            var json = await _tmdb.GetFromTmdbAsync(relativeUrl);

            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            if (!root.TryGetProperty("results", out var results) || results.GetArrayLength() == 0)
            {
                return NotFound();
            }

            return Ok(new
            {
                success = true,
                content = results.Clone()
            });
        }

        [HttpGet("movie/{page}")]
        public async Task<IActionResult> SearchMovie(int page, [FromQuery] string query, [FromQuery] string lang = "en-US", [FromQuery] int count = 20)
        {
            if (string.IsNullOrWhiteSpace(query))
                return BadRequest(new { success = false, message = "Query cannot be empty." });

            string relativeUrl = $"search/movie?query={Uri.EscapeDataString(query)}" +
                                 $"&include_adult=false&language={lang}&page={page}";

            var json = await _tmdb.GetFromTmdbAsync(relativeUrl);

            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            if (!root.TryGetProperty("results", out var resultsElement) || resultsElement.GetArrayLength() == 0)
            {
                return NotFound(new { success = false, message = "No movies found." });
            }

            var totalPages = root.GetProperty("total_pages").GetInt32();
            var totalResults = root.GetProperty("total_results").GetInt32();

            var results = resultsElement.EnumerateArray()
                                        .Take(count)
                                        .Select(x => JsonSerializer.Deserialize<object>(x.GetRawText()))
                                        .ToList();

            return Ok(new
            {
                success = true,
                query,
                page,
                totalPages,
                totalResults,
                movies = results
            });
        }

        [HttpGet("tv/{page}")]
        public async Task<IActionResult> SearchTv(int page, [FromQuery] string query, [FromQuery] string lang = "en-US", [FromQuery] int count = 20)
        {
            if (string.IsNullOrWhiteSpace(query))
                return BadRequest(new { success = false, message = "Query cannot be empty." });

            string relativeUrl = $"search/tv?query={Uri.EscapeDataString(query)}" +
                                 $"&include_adult=false&language={lang}&page={page}";

            var json = await _tmdb.GetFromTmdbAsync(relativeUrl);

            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            if (!root.TryGetProperty("results", out var resultsElement) || resultsElement.GetArrayLength() == 0)
            {
                return NotFound(new { success = false, message = "No TV shows found." });
            }

            var totalPages = root.GetProperty("total_pages").GetInt32();
            var totalResults = root.GetProperty("total_results").GetInt32();

            var results = resultsElement.EnumerateArray()
                                        .Take(count)
                                        .Select(x => JsonSerializer.Deserialize<object>(x.GetRawText()))
                                        .ToList();

            return Ok(new
            {
                success = true,
                query,
                page,
                totalPages,
                totalResults,
                tvShows = results
            });
        }
    }
}
