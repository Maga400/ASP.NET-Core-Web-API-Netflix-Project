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
            var results = root.GetProperty("results");

            if (results.GetArrayLength() == 0)
            {
                return NotFound();
            }

            return Ok(new
            {
                success = true,
                content = results
            });
        }

        [HttpGet("movie/{query}")]
        public async Task<IActionResult> SearchMovie(string query, [FromQuery] string lang = "en-US")
        {
            var relativeUrl = $"search/movie?query={Uri.EscapeDataString(query)}&include_adult=false&language={lang}&page=1";
            var json = await _tmdb.GetFromTmdbAsync(relativeUrl);

            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;
            var results = root.GetProperty("results");

            if (results.GetArrayLength() == 0)
            {
                return NotFound();
            }

            return Ok(new
            {
                success = true,
                content = results
            });
        }

        [HttpGet("tv/{query}")]
        public async Task<IActionResult> SearchTv(string query, [FromQuery] string lang = "en-US")
        {
            var relativeUrl = $"search/tv?query={Uri.EscapeDataString(query)}&include_adult=false&language={lang}&page=1";
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
                content = results
            });
        }
    }
}
