using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Microsoft.IdentityModel.Tokens;

using AuthenticationServer.API.Models;

namespace AuthenticationServer.API.Services.TokenGenerators
{
    public class AccessTokenGenerator
    {
        private readonly AuthConfiguration authConfiguration;

        public AccessTokenGenerator(AuthConfiguration authConfiguration)
        {
            this.authConfiguration = authConfiguration; 
        }

        public string GenerateToken(User user)
        {
            var claims = new List<Claim>()
            {
                new Claim("id", user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Username)
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(this.authConfiguration.AccessTokenSecretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                this.authConfiguration.Issuer,
                this.authConfiguration.Audience,
                claims,
                DateTime.UtcNow,
                DateTime.UtcNow.AddMinutes(this.authConfiguration.AccessTokenExpirationMinutes),
                credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
