using Dysnomia.CoursFrontM1.GamesDb.Common.Dao;

namespace Dysnomia.CoursFrontM1.GamesDb.Common.Dto {
    public class UserDto {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public List<IGDB.Models.Game> Favorites { get; set; } = null!;
    }

    public static class UserDtoExtensions {
        public static UserDto ToDTO(this UserDao user) {
            return new UserDto {
                Id = user.Id,
                Name = user.Name,
            };
        }
    }
}
