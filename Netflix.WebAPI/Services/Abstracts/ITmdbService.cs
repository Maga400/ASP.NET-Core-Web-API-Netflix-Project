namespace Netflix.WebAPI.Services.Abstracts
{
    public interface ITmdbService
    {
        Task<string> GetFromTmdbAsync(string relativeUrl);
    }
}
