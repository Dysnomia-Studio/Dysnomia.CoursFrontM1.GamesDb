namespace Dysnomia.CoursFrontM1.GamesDb.Common.Dao {
    public class UserDao {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string HashedPassword { get; set; } = null!;
        public List<ulong> Favorites { get; set; } = null!;
    }
}
