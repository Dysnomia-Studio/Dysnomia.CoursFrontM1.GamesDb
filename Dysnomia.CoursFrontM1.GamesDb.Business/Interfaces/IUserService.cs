using Dysnomia.CoursFrontM1.GamesDb.Common.Dto;
using Dysnomia.CoursFrontM1.GamesDb.Common.Requests;

namespace Dysnomia.CoursFrontM1.GamesDb.Business.Interfaces {
    public interface IUserService {
        Task AddGameToFavorites(string? name, ulong gameId);
        Task<string?> AuthenticateUser(string username, string password);
        Task<UserDto?> GetByUsername(string? name);
        Task<string> Register(UserRegistrationRequest registrationRequest);
        Task RemoveGameFromFavorites(string? name, ulong gameId);
        string RenewToken(string name);
        Task DeleteAccount(string name);
    }
}
