using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Netflix.Business.Services.Abstracts;
using Netflix.Entities.Models;

namespace Netflix.WebAPI.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class FavouriteController : ControllerBase
    {
        private readonly IFavouriteService _favouriteService;
        public FavouriteController(IFavouriteService favouriteService)
        {
            _favouriteService = favouriteService;
        }

        [HttpGet("allFavourites")]
        public async Task<IActionResult> GetAllFavourites()
        {
            var favourites = await _favouriteService.GetAllAsync();
            if (favourites == null || !favourites.Any())
            {
                return NotFound(new { Status = "Error", Message = "No favourites found." });
            }

            return Ok(favourites);
        }

        [HttpGet("userFavourites/{userId}")]
        public async Task<IActionResult> GetUserFavourites(int userId)
        {
            var favourites = await _favouriteService.GetUserFavouritesAsync(userId);
            if (favourites == null || !favourites.Any())
            {
                return NotFound(new { Status = "Error", Message = "No favourites found for the specified user." });
            }
            return Ok(favourites);
        }

        [HttpGet("favourite/{id}")]
        public async Task<IActionResult> GetFavouriteById(int id)
        {
            var favourite = await _favouriteService.GetByIdAsync(id);
            if (favourite == null)
            {
                return NotFound(new { Status = "Error", Message = "Favourite not found." });
            }
            return Ok(favourite);
        }

        [HttpGet("favouriteByMovieId/{movieId}")]
        public async Task<IActionResult> GetFavouriteByMovieId(int movieId)
        {
            var favourite = await _favouriteService.GetByMovieIdAsync(movieId);
            if (favourite == null)
            {
                return NotFound(new { Status = "Error", Message = "Favourite not found for the specified movie." });
            }
            return Ok(favourite);
        }

        [HttpGet("favouritesByType/{type}")]
        public async Task<IActionResult> GetFavouritesByType(string type)
        {
            var favourites = await _favouriteService.GetByTypeAsync(type);
            if (favourites == null || !favourites.Any())
            {
                return NotFound(new { Status = "Error", Message = "No favourites found for the specified type." });
            }
            return Ok(favourites);
        }

        [HttpPost("addFavourite")]
        public async Task<IActionResult> AddFavourite([FromBody] Favourite favourite)
        {
            if (favourite == null)
            {
                return BadRequest(new { Status = "Error", Message = "Invalid favourite data." });
            }
            await _favouriteService.AddAsync(favourite);
            return CreatedAtAction(nameof(GetFavouriteById), new { id = favourite.Id }, favourite);
        }

        [HttpPut("updateFavourite/{id}")]
        public async Task<IActionResult> UpdateFavourite(int id, [FromBody] Favourite favourite)
        {
            if (favourite == null || favourite.Id != id)
            {
                return BadRequest(new { Status = "Error", Message = "Invalid favourite data." });
            }
            var existingFavourite = await _favouriteService.GetByIdAsync(id);
            if (existingFavourite == null)
            {
                return NotFound(new { Status = "Error", Message = "Favourite not found." });
            }
            await _favouriteService.UpdateAsync(favourite);
            return NoContent();
        }

        [HttpDelete("deleteFavourite/{id}")]
        public async Task<IActionResult> DeleteFavourite(int id)
        {
            var favourite = await _favouriteService.GetByIdAsync(id);
            if (favourite == null)
            {
                return NotFound(new { Status = "Error", Message = "Favourite not found." });
            }
            await _favouriteService.DeleteAsync(favourite);
            return NoContent();
        }
    }
}
