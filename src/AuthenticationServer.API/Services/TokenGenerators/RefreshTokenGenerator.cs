using System.Security;

using AuthenticationServer.API.Models;

namespace AuthenticationServer.API.Services.TokenGenerators
{
    public class RefreshTokenGenerator
    {
        private readonly AuthConfiguration authConfiguration;
        private readonly TokenGenerator tokenGenerator;

        public RefreshTokenGenerator(AuthConfiguration authConfiguration, TokenGenerator tokenGenerator)
        {
            this.authConfiguration = authConfiguration;
            this.tokenGenerator = tokenGenerator;
        }

        public string GenerateRefreshToken()
        {
            return this.tokenGenerator.GenerateToken(
                secretKey: this.authConfiguration.RefreshTokenSecretKey,
                issuer: this.authConfiguration.Issuer,
                audience: this.authConfiguration.Audience,
                tokenExpirationMinutes: this.authConfiguration.RefreshTokenExpirationMinutes);
        }
    }
}
