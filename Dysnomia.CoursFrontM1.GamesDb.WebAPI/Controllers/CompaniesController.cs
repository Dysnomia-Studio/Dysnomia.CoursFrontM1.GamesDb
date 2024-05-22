using Dysnomia.CoursFrontM1.GamesDb.Business.Interfaces;
using Dysnomia.CoursFrontM1.GamesDb.Common;

using IGDB.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dysnomia.CoursFrontM1.GamesDb.WebAPI.Controllers {
    /// <summary>
    /// Get companies reference data
    /// </summary>
    [Authorize(Policy = AuthPolicies.JWT_POLICY)]
    [ApiController]
    [Route("api/[controller]")]
    public class CompaniesController : ControllerBase {
        private readonly ICompaniesService companiesService;
        public CompaniesController(ICompaniesService companiesService) {
            this.companiesService = companiesService;
        }

        /// <summary>
        /// (Needs auth) Get a company by its unique id (.involvedCompanies.ids items in a game object)
        /// </summary>
        /// <param name="id">The id to seek</param>
        /// <returns>Company informations</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<Game>> GetCompanyById(int id) {
            return Ok(await companiesService.GetCompanyByIdAsync(id));
        }
    }
}
