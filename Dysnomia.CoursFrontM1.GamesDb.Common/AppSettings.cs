namespace Dysnomia.CoursFrontM1.GamesDb.Common {
    public class AppSettings {
        public string ConnectionString { get; init; } = null!;
        public string IGDBClientId { get; init; } = null!;
        public string IGDBClientSecret { get; init; } = null!;
        public string JwtIssuer { get; set; } = null!;
        public string JwtKey { get; set; } = null!;
    }
}
