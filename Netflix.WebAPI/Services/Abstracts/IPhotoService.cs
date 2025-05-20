using Netflix.WebAPI.Dtos;

namespace Netflix.WebAPI.Services.Abstracts
{
    public interface IPhotoService
    {
        Task<string> UploadImageAsync(PhotoCreationDto dto);
    }
}
