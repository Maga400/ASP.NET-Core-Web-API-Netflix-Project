using Microsoft.EntityFrameworkCore;
using Netflix.DataAccess.Data;
using Netflix.DataAccess.Repositories.Abstracts;
using Netflix.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Netflix.DataAccess.Repositories.Concretes
{
    public class FavouriteDAL : IFavouriteDAL
    {
        private readonly NetflixDbContext _context;
        public FavouriteDAL(NetflixDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Favourite fav)
        {
            await _context.Favourites.AddAsync(fav);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(Favourite fav)
        {
            _context.Favourites.Remove(fav);
            await _context.SaveChangesAsync();
        }
        public async Task<List<Favourite>> GetAllAsync()
        {
            return await _context.Favourites.ToListAsync();
        }
        public async Task<Favourite> GetByIdAsync(int id)
        {
            return await _context.Favourites.FirstOrDefaultAsync(x => x.Id == id);
        }
        public async Task<Favourite> GetByMovieIdAsync(int id)
        {
            return await _context.Favourites.FirstOrDefaultAsync(x => x.MovieId == id);
        }
        public async Task<List<Favourite>> GetByTypeAsync(string type)
        {
            return await _context.Favourites.Where(f => f.Type == type).ToListAsync();
        }
        public async Task<List<Favourite>> GetUserFavouritesAsync(int userId)
        {
            return await _context.Favourites.Where(f => f.UserId == userId).ToListAsync();
        }
        public async Task UpdateAsync(Favourite fav)
        {
            _context.Favourites.Update(fav);
            await _context.SaveChangesAsync();
        }
    }
}
