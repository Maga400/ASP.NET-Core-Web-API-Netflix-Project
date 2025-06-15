using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Netflix.Entities.Models
{
    public class Favourite
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public CustomIdentityUser? User { get; set; }
        public int MovieId { get; set; }
        public string? Type { get; set; }
    }
}
