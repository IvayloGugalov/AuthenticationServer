using System.Threading.Tasks;

using AuthenticationServer.API.Helpers;
using AuthenticationServer.API.Models;
using AuthenticationServer.API.Models.Responses;
using AuthenticationServer.API.Services.Repositories;
using AuthenticationServer.API.Services.TokenGenerators;

namespace AuthenticationServer.API.Services.Authenticators
{
    public class Authenticator
    {
        private readonly AccessTokenGenerator accessTokenGenerator;
        private readonly RefreshTokenGenerator refreshTokenGenerator;
        private readonly IRefreshTokenRepository refreshTokenRepository;


        public Authenticator(AccessTokenGenerator accessTokenGenerator, RefreshTokenGenerator refreshTokenGenerator, IRefreshTokenRepository refreshTokenRepository)
        {
            this.accessTokenGenerator = accessTokenGenerator;
            this.refreshTokenGenerator = refreshTokenGenerator;
            this.refreshTokenRepository = refreshTokenRepository;
        }

        public async Task<UserAuthenticatedResponse> AuthenticateUserAsync(User user)
        {
            var accessTokenValue = this.accessTokenGenerator.GenerateAccessToken(user);
            var refreshTokenValue = this.refreshTokenGenerator.GenerateRefreshToken();

            var refreshToken = new RefreshToken()
            {
                Token = refreshTokenValue,
                UserId = user.Id
            };

            await this.refreshTokenRepository.Create(refreshToken);

            return new UserAuthenticatedResponse()
            {
                AccessTokenValue = accessTokenValue,
                RefreshTokenValue = refreshTokenValue
            };
        }
    }
}
