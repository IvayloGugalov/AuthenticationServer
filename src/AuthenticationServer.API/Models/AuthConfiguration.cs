﻿namespace AuthenticationServer.API.Models
{
    public class AuthConfiguration
    {
        public string AccessTokenSecretKey { get; set; }
        public int AccessTokenExpirationMinutes { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
    }
}
