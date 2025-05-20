using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Netflix.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Netflix.DataAccess.Data
{
    public class NetflixDbContext : IdentityDbContext<CustomIdentityUser,CustomIdentityRole,string>
    {
        public NetflixDbContext(DbContextOptions<NetflixDbContext> options) : base(options)
        {
        
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=NetflixDB;Integrated Security=True;", b => b.MigrationsAssembly("Netflix.WebAPI"));
        }
    }
}
