﻿using System.ComponentModel.DataAnnotations;

namespace AuthenticationServer.API.Models.Request
{
    public class LoginRequest
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
