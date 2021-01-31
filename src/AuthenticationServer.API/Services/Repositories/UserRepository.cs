using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AuthenticationServer.API.Models;

namespace AuthenticationServer.API.Services.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly List<User> users = new List<User>();

        public Task<User> Create(User user)
        {
            user.Id = Guid.NewGuid();
            this.users.Add(user);

            return Task.FromResult(user);
        }

        public Task<User> GetByEmail(string email)
        {
            return Task.FromResult(this.users.FirstOrDefault(u => u.Email == email));
        }

        public Task<User> GetByUserName(string username)
        {
            return Task.FromResult(this.users.FirstOrDefault(u => u.Username == username));
        }
    }
}
