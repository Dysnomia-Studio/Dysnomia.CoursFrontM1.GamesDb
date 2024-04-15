using Dysnomia.CoursFrontM1.GamesDb.Business.Interfaces;
using Dysnomia.CoursFrontM1.GamesDb.Common;
using Dysnomia.CoursFrontM1.GamesDb.Common.Dto;
using Dysnomia.CoursFrontM1.GamesDb.Common.Requests;
using Dysnomia.CoursFrontM1.GamesDb.DataAccess.Interfaces;

using IGDB.Models;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Dysnomia.CoursFrontM1.GamesDb.Business.Implementations {
    public class UserService : IUserService {
        private readonly IUserDataAccess _userDataAccess;
        private readonly IGameService _gameService;
        private readonly ILogger<UserService> _logger;
        private readonly string JwtKey;
        private readonly string JwtIssuer;

        public UserService(IUserDataAccess userDataAccess, IGameService gameService, ILogger<UserService> logger, IOptions<AppSettings> options) {
            _userDataAccess = userDataAccess;
            _gameService = gameService;
            _logger = logger;

            this.JwtKey = options.Value.JwtKey;
            this.JwtIssuer = options.Value.JwtIssuer;
        }

        private string GenerateJwtToken(string username) {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            List<Claim> claims = [new Claim(ClaimTypes.Name, username)];

            var token = new JwtSecurityToken(JwtIssuer,
              JwtIssuer,
              claims,
              expires: DateTime.Now.AddMinutes(120),
              signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<string?> AuthenticateUser(string username, string password) {
            try {
                var user = await _userDataAccess.GetByUsername(username);
                if (user == default) {
                    return null;
                }

                if (!BCrypt.Net.BCrypt.Verify(password, user.HashedPassword)) {
                    return null;
                }

                return GenerateJwtToken(username);
            } catch (Exception e) {
                _logger.LogError(e, "Error when authenticating the user");

                throw;
            }
        }

        public async Task<UserDto?> GetByUsername(string? name) {
            try {
                var user = await _userDataAccess.GetByUsername(name);
                if (user is null) {
                    return null;
                }

                List<Game> favorites = [];
                foreach (var id in user.Favorites) {
                    favorites.Add(
                        await _gameService.GetGameById(id)
                    );
                }

                return new UserDto {
                    Id = user.Id,
                    Name = user.Name,
                    Favorites = favorites
                };
            } catch (Exception e) {
                _logger.LogError(e, $"Error when getting an user by its username: \"{name}\"");

                throw;
            }
        }

        public async Task<string> Register(UserRegistrationRequest registrationRequest) {
            try {
                if (registrationRequest == null) {
                    throw new InvalidDataException("Invalid json");
                }

                if (string.IsNullOrWhiteSpace(registrationRequest.Username)) {
                    throw new InvalidDataException("Invalid username, should be > 3 characters long");
                }
                registrationRequest.Username = registrationRequest.Username.Trim();

                if (registrationRequest.Username.Length < 4) {
                    throw new InvalidDataException("Invalid username, should be > 3 characters long");
                }

                var otherUser = await _userDataAccess.GetByUsername(registrationRequest.Username);
                if (otherUser is not null) {
                    throw new InvalidDataException("Invalid username, already existing");
                }

                if (registrationRequest.Password != registrationRequest.ConfirmationPassword) {
                    throw new InvalidDataException("Password and confirmation doesn't match");
                }

                registrationRequest.Password = BCrypt.Net.BCrypt.HashPassword(registrationRequest.Password);

                await _userDataAccess.Register(registrationRequest);

                return GenerateJwtToken(registrationRequest.Username);
            } catch (Exception e) {
                _logger.LogError(e, $"Error when trying to register an user");

                throw;
            }
        }

        public string RenewToken(string name) {
            try {
                return GenerateJwtToken(name);
            } catch (Exception e) {
                _logger.LogError(e, $"Error when renewing user token: \"{name}\"");

                throw;
            }
        }

        public async Task AddGameToFavorites(string? name, ulong gameId) {
            try {
                var user = await _userDataAccess.GetByUsername(name);
                if (user is null) {
                    throw new Exception();
                }

                if (user.Favorites.Contains(gameId)) {
                    throw new InvalidDataException();
                }

                user.Favorites.Add(gameId);

                await _userDataAccess.PersistChangesAsync();
            } catch (Exception e) {
                _logger.LogError(e, $"Error when trying to register an user");

                throw;
            }
        }

        public async Task RemoveGameFromFavorites(string? name, ulong gameId) {
            try {
                var user = await _userDataAccess.GetByUsername(name);
                if (user is null) {
                    throw new Exception();
                }

                user.Favorites.Remove(gameId);

                await _userDataAccess.PersistChangesAsync();
            } catch (Exception e) {
                _logger.LogError(e, $"Error when trying to register an user");

                throw;
            }
        }
    }
}
