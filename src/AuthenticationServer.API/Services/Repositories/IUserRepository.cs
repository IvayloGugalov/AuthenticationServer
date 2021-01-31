using System.Threading.Tasks;

using AuthenticationServer.API.Models;

namespace AuthenticationServer.API.Services.Repositories
{
    public interface IUserRepository
    {
        Task<User> GetByEmail(string email);
        Task<User> GetByUserName(string username);
        Task<User> Create(User user);
    }
}
