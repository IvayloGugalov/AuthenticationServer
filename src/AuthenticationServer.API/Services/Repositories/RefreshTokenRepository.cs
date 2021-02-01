using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AuthenticationServer.API.Helpers;
using AuthenticationServer.API.Models;

namespace AuthenticationServer.API.Services.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly List<RefreshToken> refreshTokens = new List<RefreshToken>();

        public Task Create(RefreshToken refreshToken)
        {
            refreshToken.Id = Guid.NewGuid();

            this.refreshTokens.Add(refreshToken);

            return Task.CompletedTask;
        }

        public Task Delete(Guid id)
        {
            this.refreshTokens.RemoveAll(rt => rt.Id == id);

            return Task.CompletedTask;
        }

        public Task DeleteAll(Guid userId)
        {
            this.refreshTokens.RemoveAll(rt => rt.UserId == userId);

            return Task.CompletedTask;
        }

        public Task<RefreshToken> GetByTokenValue(string tokenValue)
        {
            var refreshToken = this.refreshTokens.FirstOrDefault(rt => rt.Token == tokenValue);

            return Task.FromResult(refreshToken);
        }
    }
}
