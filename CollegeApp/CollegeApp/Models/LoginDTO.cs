﻿using System.ComponentModel.DataAnnotations;

namespace CollegeApp.Models
{
    public class LoginDTO
    {
        [Required]
        public string PolicyName { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
