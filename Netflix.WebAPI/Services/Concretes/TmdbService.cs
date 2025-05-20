using Netflix.WebAPI.Services.Abstracts;
using System.Net.Http.Headers;

namespace Netflix.WebAPI.Services.Concretes
{
    public class TmdbService : ITmdbService
    {
        private readonly HttpClient _http;
        private readonly IConfiguration _config;
        public TmdbService(HttpClient http, IConfiguration config)
        {
            _http = http;
            _config = config;
            _http.BaseAddress = new Uri("https://api.themoviedb.org/3/");
            _http.DefaultRequestHeaders.Accept.Clear();
            _http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var token = _config["TMDB:BearerToken"];
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
        public async Task<string> GetFromTmdbAsync(string relativeUrl)
        {
            var response = await _http.GetAsync(relativeUrl);

            if (!response.IsSuccessStatusCode)
            {
                var msg = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"TMDB API error: {response.StatusCode}, {msg}");
            }

            return await response.Content.ReadAsStringAsync();
        }
    }
}
