﻿using Dysnomia.CoursFrontM1.GamesDb.Business.Interfaces;
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

        private static readonly Dictionary<(uint, uint), (DateTime, Game[])> getGamesCache = [];
        public async Task<Game[]> GetGames(uint pageSize, uint page) {
            if (pageSize < 5 || pageSize > 500) {
                throw new InvalidDataException("Page size must be between 5 and 500");
            }

            if (page < 1) {
                throw new InvalidDataException("Page number must greater or equal to one");
            }

            if (getGamesCache.TryGetValue((pageSize, page), out var gameCache) && gameCache.Item1 - DateTime.UtcNow < TimeSpan.FromDays(1)) {
                return gameCache.Item2;
            }

            var request = $"fields *; sort total_rating desc; where rating != null & total_rating_count > 50; limit {pageSize};";
            if (page > 1) {
                request += $"offset {pageSize * (page - 1)};";
            }

            var games = await IGDBClient.QueryAsync<Game>(IGDBClient.Endpoints.Games, request);

            getGamesCache[(pageSize, page)] = (DateTime.UtcNow, games);

            return games;
        }

        private static readonly Dictionary<(string, uint, uint), (DateTime, Game[])> searchGamesCache = [];
        public async Task<Game[]> SearchGames(string term, uint pageSize, uint page) {
            if (pageSize < 1 || pageSize > 500) {
                throw new InvalidDataException("Page size must be between 5 and 500");
            }

            if (page < 1) {
                throw new InvalidDataException("Page number must greater or equal to one");
            }

            if (searchGamesCache.TryGetValue((term, pageSize, page), out var gameCache) && gameCache.Item1 - DateTime.UtcNow < TimeSpan.FromDays(1)) {
                return gameCache.Item2;
            }

            var request = $"fields *; search \"{term}\"; limit {pageSize};";
            if (page > 1) {
                request += $"offset {pageSize * (page - 1)};";
            }

            var games = await IGDBClient.QueryAsync<Game>(IGDBClient.Endpoints.Games, request);

            searchGamesCache[(term, pageSize, page)] = (DateTime.UtcNow, games);

            return games;
        }

        private static readonly Dictionary<ulong, (DateTime, Game)> gamesCache = [];
        public async Task<Game?> GetGameById(ulong id) {
            if (gamesCache.TryGetValue(id, out var gameCache) && gameCache.Item1 - DateTime.UtcNow < TimeSpan.FromDays(1)) {
                return gameCache.Item2;
            }

            var request = $"fields *; sort rating desc; where id = {id};";

            var games = await IGDBClient.QueryAsync<Game>(IGDBClient.Endpoints.Games, request);

            var game = games.FirstOrDefault();
            if (game is not null) {
                gamesCache[id] = (DateTime.UtcNow, game);
            }

            return game;
        }

        private static readonly Dictionary<ulong, (DateTime, Screenshot[])> gamesScreenshotsCache = [];
        public async Task<Screenshot[]> GetGameScreenshots(ulong id) {
            if (gamesScreenshotsCache.TryGetValue(id, out var gameScreenshotsCache) && gameScreenshotsCache.Item1 - DateTime.UtcNow < TimeSpan.FromDays(1)) {
                return gameScreenshotsCache.Item2;
            }

            var request = $"fields *; sort rating desc; where game = {id};";

            var screenshots = (await IGDBClient.QueryAsync<Screenshot>(
                IGDBClient.Endpoints.Screenshots,
                request
            )).Select(x => {
                x.Url = "https:" + x.Url.Replace("t_thumb", "t_original");

                return x;
            }).ToArray();

            gamesScreenshotsCache[id] = (DateTime.UtcNow, screenshots);

            return screenshots;
        }

        public async Task<Platform[]> GetGamePlatforms(IEnumerable<int> ids) {
            if (!ids.Any()) {
                return [];
            }

            var request = $"fields *; where id = ({string.Join(',', ids)});";

            return await IGDBClient.QueryAsync<Platform>(
                IGDBClient.Endpoints.Platforms,
                request
            );
        }

        private static readonly Dictionary<ulong, (DateTime, Cover[])> gamesCoversCache = [];
        public async Task<Cover[]> GetGameCovers(ulong id) {
            if (gamesCoversCache.TryGetValue(id, out var gamesCoverCache) && gamesCoverCache.Item1 - DateTime.UtcNow < TimeSpan.FromDays(1)) {
                return gamesCoverCache.Item2;
            }

            var request = $"fields *; where game = {id};";

            var covers = (await IGDBClient.QueryAsync<Cover>(
                IGDBClient.Endpoints.Covers,
                request
            )).Select(x => {
                x.Url = "https:" + x.Url.Replace("t_thumb", "t_original");

                return x;
            }).ToArray();

            gamesCoversCache[id] = (DateTime.UtcNow, covers);

            return covers;
        }
    }
}
