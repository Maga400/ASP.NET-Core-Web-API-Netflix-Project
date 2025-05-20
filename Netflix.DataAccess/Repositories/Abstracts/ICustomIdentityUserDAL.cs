using Netflix.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Netflix.DataAccess.Repositories.Abstracts
{
    public interface ICustomIdentityUserDAL
    {
        Task<List<CustomIdentityUser>> GetAllAsync();
        Task<CustomIdentityUser> GetByIdAsync(string id);
        Task<CustomIdentityUser> GetByUsernameAsync(string name);
        Task<CustomIdentityUser> GetByEmailAsync(string email);
        Task AddAsync(CustomIdentityUser user);
        Task UpdateAsync(CustomIdentityUser user);
        Task DeleteAsync(CustomIdentityUser user);
    }
}
