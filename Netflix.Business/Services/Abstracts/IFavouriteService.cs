using Netflix.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Netflix.Business.Services.Abstracts
{
    public interface IFavouriteService
    {
        Task<List<Favourite>> GetAllAsync();
        Task<List<Favourite>> GetUserFavouritesAsync(int userId);
        Task<Favourite> GetByIdAsync(int id);
        Task<Favourite> GetByMovieIdAsync(int id);
        Task<List<Favourite>> GetByTypeAsync(string type);
        Task AddAsync(Favourite fav);
        Task UpdateAsync(Favourite fav);
        Task DeleteAsync(Favourite fav);
    }
}
