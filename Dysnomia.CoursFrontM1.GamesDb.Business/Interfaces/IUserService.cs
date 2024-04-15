using Dysnomia.CoursFrontM1.GamesDb.Common.Dto;
using Dysnomia.CoursFrontM1.GamesDb.Common.Requests;

namespace Dysnomia.CoursFrontM1.GamesDb.Business.Interfaces {
    public interface IUserService {
        Task<string?> AuthenticateUser(string username, string password);
        Task<UserDto?> GetByUsername(string? name);
        Task<string> Register(UserRegistrationRequest registrationRequest);
        string RenewToken(string name);
    }
}
