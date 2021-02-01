using System;
using System.Threading.Tasks;

using AuthenticationServer.API.Models;

namespace AuthenticationServer.API.Services.Repositories
{
    public interface IRefreshTokenRepository
    {
        Task Create(RefreshToken refreshToken);
        Task Delete(Guid id);
        Task<RefreshToken> GetByTokenValue(string tokenValue);
        Task DeleteAll(Guid userId);
    }
}
