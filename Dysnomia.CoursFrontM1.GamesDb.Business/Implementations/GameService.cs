using Dysnomia.CoursFrontM1.GamesDb.Business.Interfaces;
using Dysnomia.CoursFrontM1.GamesDb.Common;

using IGDB;
using IGDB.Models;

using Microsoft.Extensions.Options;

namespace Dysnomia.CoursFrontM1.GamesDb.Business.Implementations {
    public class GameService : IGameService {
        private readonly IGDBClient IGDBClient;
        public GameService(IOptions<AppSettings> options) {
            this.IGDBClient = new IGDBClient(options?.Value?.IGDBClientId, options?.Value?.IGDBClientSecret);
        }

        public async Task<Game[]> GetGames(uint pageSize, uint page) {
            if (pageSize < 5 || pageSize > 500) {
                throw new InvalidDataException("Page size must be between 5 and 500");
            }

            if (page < 1) {
                throw new InvalidDataException("Page number must greater or equal to one");
            }

            var request = $"fields *; sort rating desc; where rating != null; limit {pageSize};";
            if (page > 1) {
                request += $"offset {pageSize * (page - 1)};";
            }

            var games = await IGDBClient.QueryAsync<Game>(IGDBClient.Endpoints.Games, request);

            return games;
        }

        public async Task<Game[]> SearchGames(string term, uint pageSize, uint page) {
            if (pageSize < 5 || pageSize > 500) {
                throw new InvalidDataException("Page size must be between 5 and 500");
            }

            if (page < 1) {
                throw new InvalidDataException("Page number must greater or equal to one");
            }

            var request = $"fields *; search \"{term}\"; limit {pageSize};";
            if (page > 1) {
                request += $"offset {pageSize * (page - 1)};";
            }

            var games = await IGDBClient.QueryAsync<Game>(IGDBClient.Endpoints.Games, request);

            return games;
        }

        public async Task<Game?> GetGameById(ulong id) {
            var request = $"fields *; sort rating desc; where id = {id};";

            var games = await IGDBClient.QueryAsync<Game>(IGDBClient.Endpoints.Games, request);

            return games.FirstOrDefault();
        }
    }
}