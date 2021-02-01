using System.Collections.Generic;
using System.Security;
using System.Security.Claims;

using AuthenticationServer.API.Models;

namespace AuthenticationServer.API.Services.TokenGenerators
{
    public class AccessTokenGenerator
    {
        private readonly AuthConfiguration authConfiguration;
        private readonly TokenGenerator tokenGenerator;

        public AccessTokenGenerator(AuthConfiguration authConfiguration, TokenGenerator tokenGenerator)
        {
            this.authConfiguration = authConfiguration;
            this.tokenGenerator = tokenGenerator;
        }

        public string GenerateAccessToken(User user)
        {
            var claims = new List<Claim>()
            {
                new Claim("id", user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Username)
            };

            return this.tokenGenerator.GenerateToken(
                secretKey: this.authConfiguration.AccessTokenSecretKey,
                issuer: this.authConfiguration.Issuer,
                audience: this.authConfiguration.Audience,
                tokenExpirationMinutes: this.authConfiguration.AccessTokenExpirationMinutes,
                claims: claims);
        }
    }
}
