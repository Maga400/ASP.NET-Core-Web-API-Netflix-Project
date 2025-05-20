﻿using System.ComponentModel.DataAnnotations;

namespace Netflix.WebAPI.Dtos
{
    public class RegisterDto
    {
        [Required]
        public string? Username { get; set; }
        [Required]
        public string? Email { get; set; }
        [Required]
        public string? Password { get; set; }
        [Required]
        public string? Role { get; set; } = "User";

        //[Required]
        //public string? Name { get; set; }
        //[Required]
        //public string? Surname { get; set; }
        //[Required]
        //public string? ImagePath { get; set; }
        //[Required]
        //public string? CarType { get; set; }
        //[Required]
        //public string? BankName { get; set; }
        //[Required]
        //public string? CardNumber { get; set; }
        //[Required]
        //public string? PhoneNumber { get; set; }
    }
}
