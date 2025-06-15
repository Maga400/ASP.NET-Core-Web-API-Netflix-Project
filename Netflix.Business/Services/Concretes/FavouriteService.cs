using Netflix.Business.Services.Abstracts;
using Netflix.DataAccess.Repositories.Abstracts;
using Netflix.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Netflix.Business.Services.Concretes
{
    public class FavouriteService : IFavouriteService
    {
        private readonly IFavouriteDAL _favouriteDAL;
        public FavouriteService(IFavouriteDAL favouriteDAL)
        {
            _favouriteDAL = favouriteDAL;
        }
        public async Task AddAsync(Favourite fav)
        {
            await _favouriteDAL.AddAsync(fav);
        }
        public async Task DeleteAsync(Favourite fav)
        {
            await _favouriteDAL.DeleteAsync(fav);
        }
        public async Task<List<Favourite>> GetAllAsync()
        {
            return await _favouriteDAL.GetAllAsync();
        }
        public async Task<Favourite> GetByIdAsync(int id)
        {
            return await _favouriteDAL.GetByIdAsync(id);
        }
        public async Task<Favourite> GetByMovieIdAsync(int id)
        {
            return await _favouriteDAL.GetByMovieIdAsync(id);
        }
        public async Task<List<Favourite>> GetByTypeAsync(string type)
        {
            return await _favouriteDAL.GetByTypeAsync(type);
        }
        public async Task<List<Favourite>> GetUserFavouritesAsync(int userId)
        {
            return await _favouriteDAL.GetUserFavouritesAsync(userId);
        }

        public async Task UpdateAsync(Favourite fav)
        {
            await _favouriteDAL.UpdateAsync(fav);
        }
    }
}
