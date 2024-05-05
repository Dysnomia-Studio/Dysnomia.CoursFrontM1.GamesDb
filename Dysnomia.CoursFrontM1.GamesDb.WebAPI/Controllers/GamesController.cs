using Dysnomia.CoursFrontM1.GamesDb.Business.Interfaces;
using Dysnomia.CoursFrontM1.GamesDb.Common;

using IGDB.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dysnomia.CoursFrontM1.GamesDb.WebAPI.Controllers {
    /// <summary>
    /// Get games reference data
    /// </summary>
    [Authorize(Policy = AuthPolicies.JWT_POLICY)]
    [ApiController]
    [Route("api/[controller]")]
    public class GamesController : ControllerBase {
        private readonly IGameService gameService;
        public GamesController(IGameService gameService) {
            this.gameService = gameService;
        }

        /// <summary>
        /// (No auth) Get games ordered by positive review % descending, reviews are weighted which means number of votes is also taken in account
        /// </summary>
        /// <param name="pageSize">Number of items per result page, default is 10, max is 500</param>
        /// <param name="page">Page number to fetch, default is 1</param>
        /// <returns>A list of games</returns>
        [AllowAnonymous]
        [HttpGet("top")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Game[]>> GetGames(uint pageSize = 10, uint page = 1) {
            try {
                return Ok(await gameService.GetGames(pageSize, page));
            } catch (InvalidDataException ex) {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// (Needs auth) Get games matching a specific string
        /// </summary>
        /// <param name="term">String to search</param>
        /// <param name="pageSize">Number of items per result page, default is 10, max is 500</param>
        /// <param name="page">Page number to fetch, default is 1</param>
        /// <returns>A list of games</returns>
        [HttpGet("search")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<Game[]>> SearchGames(string term, uint pageSize = 10, uint page = 1) {
            try {
                return Ok(await gameService.SearchGames(term, pageSize, page));
            } catch (InvalidDataException ex) {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// (Needs auth) Get a game by its unique id (.id field in a game object)
        /// </summary>
        /// <param name="id">The id to seek</param>
        /// <returns>Game informations</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<Game>> GetGameById(ulong id) {
            return Ok(await gameService.GetGameById(id));
        }

        /// <summary>
        /// (No Auth) Get screenshots for a specific game 
        /// </summary>
        /// <param name="id">Game id (.id field in a game object)</param>
        /// <returns>Screenshot list</returns>
        [AllowAnonymous]
        [HttpGet("screenshots/{id}")]
        [ProducesResponseType(typeof(Screenshot[]), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult<Screenshot[]>> GetGameScreenshots(ulong id) {
            var screenshots = await gameService.GetGameScreenshots(id);
            if (screenshots.Any()) {
                return Ok(screenshots);
            }
            return NoContent();
        }

        /// <summary>
        /// (Needs auth) Get games platforms by their ids
        /// </summary>
        /// <param name="id">Plaform ids (.platforms.ids field in a game object)</param>
        /// <returns>Platform list</returns>
        [AllowAnonymous]
        [HttpPost("platforms")]
        [ProducesResponseType(typeof(Screenshot[]), StatusCodes.Status200OK)]
        public async Task<ActionResult<Platform[]>> GetGamePlatforms(IEnumerable<int> ids) {
            return Ok(await gameService.GetGamePlatforms(ids));
        }

        /// <summary>
        /// (No Auth) Get cover pictures for a specific game 
        /// </summary>
        /// <param name="id">Game id (.id field in a game object)</param>
        /// <returns>Cover list</returns>
        [AllowAnonymous]
        [HttpGet("covers/{id}")]
        [ProducesResponseType(typeof(Screenshot[]), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult<Screenshot[]>> GetGameCovers(ulong id) {
            var covers = await gameService.GetGameCovers(id);
            if (covers.Any()) {
                return Ok(covers);
            }
            return NoContent();
        }
    }
}
