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
    public class CustomIdentityUserDAL : ICustomIdentityUserDAL
    {
        private readonly NetflixDbContext _context;
        public CustomIdentityUserDAL(NetflixDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(CustomIdentityUser user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(CustomIdentityUser user)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
        public async Task<List<CustomIdentityUser>> GetAllAsync()
        {
            return await _context.Users.ToListAsync();
        }
        public async Task<CustomIdentityUser> GetByIdAsync(string id)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
        }
        public async Task<CustomIdentityUser> GetByUsernameAsync(string name)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.UserName == name);
        }
        public async Task<CustomIdentityUser> GetByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.Email == email);
        }
        public async Task UpdateAsync(CustomIdentityUser user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
    }
}
