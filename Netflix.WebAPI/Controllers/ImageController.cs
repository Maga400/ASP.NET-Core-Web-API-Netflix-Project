using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Netflix.WebAPI.Dtos;
using Netflix.WebAPI.Services.Abstracts;

namespace Netflix.WebAPI.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly IPhotoService _photoService;
        public ImageController(IPhotoService photoService)
        {
            _photoService = photoService;
        }

        [HttpPost]
        public async Task<IActionResult> Post()
        {
            var file = Request.Form.Files.GetFile("file");

            if (file != null && file.Length > 0)
            {
                string result = await _photoService.UploadImageAsync(new PhotoCreationDto { File = file });
                return Ok(new { ImagePath = result });
            }
            return BadRequest(new { Message = "Photo Creation Failed!" });
        }
    }
}
