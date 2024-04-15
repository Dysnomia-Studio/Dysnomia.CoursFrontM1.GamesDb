namespace Dysnomia.CoursFrontM1.GamesDb.Common.Requests {
    public class UserRegistrationRequest {
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string ConfirmationPassword { get; set; } = null!;
    }
}
