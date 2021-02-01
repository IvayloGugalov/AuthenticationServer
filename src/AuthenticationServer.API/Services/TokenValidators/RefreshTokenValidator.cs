using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

using Microsoft.IdentityModel.Tokens;

using AuthenticationServer.API.Models;

namespace AuthenticationServer.API.Services.TokenValidators
{
    public class RefreshTokenValidator
    {
        private readonly AuthConfiguration authConfiguration;

        public RefreshTokenValidator(AuthConfiguration authConfiguration)
        {
            this.authConfiguration = authConfiguration;
        }

        public bool Validate(string refreshTokenValue)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters()
            {
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.authConfiguration.RefreshTokenSecretKey)),
                ValidIssuer = this.authConfiguration.Issuer,
                ValidAudience = this.authConfiguration.Audience,
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                ClockSkew = TimeSpan.Zero
            };

            try
            {
                tokenHandler.ValidateToken(refreshTokenValue, validationParameters, out SecurityToken validatedToken);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }
    }
}
