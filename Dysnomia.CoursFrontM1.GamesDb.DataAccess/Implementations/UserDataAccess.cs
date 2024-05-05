using Dysnomia.CoursFrontM1.GamesDb.Common.Dao;
using Dysnomia.CoursFrontM1.GamesDb.Common.Requests;
using Dysnomia.CoursFrontM1.GamesDb.DataAccess.Interfaces;

using Microsoft.EntityFrameworkCore;

namespace Dysnomia.CoursFrontM1.GamesDb.DataAccess.Implementations {
    public class UserDataAccess : IUserDataAccess {
        private readonly DatabaseContext context;

        public UserDataAccess(DatabaseContext context) {
            this.context = context;
        }

        public Task<UserDao?> GetByUsername(string? username) {
            return context.Users.FirstOrDefaultAsync(x => x.Name == username);
        }

        public Task PersistChangesAsync() {
            return context.SaveChangesAsync();
        }

        public async Task Register(UserRegistrationRequest registrationRequest) {
            await context.AddAsync(new UserDao() {
                Name = registrationRequest.Username,
                HashedPassword = registrationRequest.Password,
                Favorites = []
            });
            await context.SaveChangesAsync();
        }

        public async Task DeleteAccount(string name) {
            await context.Users.Where(c => c.Name == name).ExecuteDeleteAsync();
        }
    }
}
