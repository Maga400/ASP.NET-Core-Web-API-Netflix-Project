using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Netflix.Entities.Models
{
    public class CustomIdentityUser : IdentityUser
    {
        public string? ImagePath { get; set; }
    }
}
