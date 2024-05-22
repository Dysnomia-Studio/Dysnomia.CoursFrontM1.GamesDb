using IGDB.Models;

namespace Dysnomia.CoursFrontM1.GamesDb.Business.Interfaces {
    public interface ICompaniesService {
        Task<Company?> GetCompanyByIdAsync(int id);
    }
}
