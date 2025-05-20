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
    public class CustomIdentityUserService : ICustomIdentityUserService
    {
        private readonly ICustomIdentityUserDAL _userDAL;
        public CustomIdentityUserService(ICustomIdentityUserDAL userDAL)
        {
            _userDAL = userDAL;
        }
        public async Task AddAsync(CustomIdentityUser user)
        {
            await _userDAL.AddAsync(user);
        }
        public async Task DeleteAsync(CustomIdentityUser user)
        {
            await _userDAL.DeleteAsync(user);
        }
        public async Task<List<CustomIdentityUser>> GetAllAsync()
        {
            return await _userDAL.GetAllAsync();
        }
        public async Task<CustomIdentityUser> GetByIdAsync(string id)
        {
            return await _userDAL.GetByIdAsync(id);
        }
        public async Task<CustomIdentityUser> GetByUsernameAsync(string name)
        {
            return await _userDAL.GetByUsernameAsync(name);
        }
        public async Task<CustomIdentityUser> GetByEmailAsync(string email)
        {
            return await _userDAL.GetByEmailAsync(email);
        }
        public async Task UpdateAsync(CustomIdentityUser user)
        {
            await _userDAL.UpdateAsync(user);
        }
    }
}
