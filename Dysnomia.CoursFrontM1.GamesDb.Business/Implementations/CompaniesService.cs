using Dysnomia.CoursFrontM1.GamesDb.Business.Interfaces;
using Dysnomia.CoursFrontM1.GamesDb.Common;

using IGDB;
using IGDB.Models;

using Microsoft.Extensions.Options;

namespace Dysnomia.CoursFrontM1.GamesDb.Business.Implementations {
    public class CompaniesService : ICompaniesService {
        private readonly IGDBClient IGDBClient;
        public CompaniesService(IOptions<AppSettings> options) {
            this.IGDBClient = new IGDBClient(options?.Value?.IGDBClientId, options?.Value?.IGDBClientSecret);
        }

        private static readonly Dictionary<int, (DateTime, Company)> companiesCache = [];
        public async Task<Company?> GetCompanyByIdAsync(int id) {
            if (companiesCache.TryGetValue(id, out var gameCache) && gameCache.Item1 - DateTime.UtcNow < TimeSpan.FromDays(1)) {
                return gameCache.Item2;
            }

            var request = $"fields *; where id = {id};";

            var company = (await IGDBClient.QueryAsync<Company>(IGDBClient.Endpoints.Companies, request)).FirstOrDefault();
            if (company != null) {
                companiesCache[id] = (DateTime.UtcNow, company);
            }

            return company;
        }
    }
}
