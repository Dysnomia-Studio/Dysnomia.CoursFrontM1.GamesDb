using IGDB.Models;

namespace Dysnomia.CoursFrontM1.GamesDb.Business.Interfaces {
    public interface IGameService {
        Task<Game[]> GetGames(uint pageSize, uint page);
        Task<Game[]> SearchGames(string term, uint pageSize, uint page);
        Task<Game?> GetGameById(ulong id);
        Task<Screenshot[]> GetGameScreenshots(ulong id);
        Task<Platform[]> GetGamePlatforms(IEnumerable<int> ids);
        Task<Cover[]> GetGameCovers(ulong id);

    }
}
