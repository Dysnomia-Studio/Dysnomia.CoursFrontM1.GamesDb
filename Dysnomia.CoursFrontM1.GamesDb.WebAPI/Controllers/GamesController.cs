using Dysnomia.CoursFrontM1.GamesDb.Business.Interfaces;
using Dysnomia.CoursFrontM1.GamesDb.Common;

using IGDB.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dysnomia.CoursFrontM1.GamesDb.WebAPI.Controllers {
    [Authorize(Policy = AuthPolicies.JWT_POLICY)]
    [ApiController]
    [Route("api/[controller]")]
    public class GamesController : ControllerBase {
        private readonly IGameService gameService;
        public GamesController(IGameService gameService) {
            this.gameService = gameService;
        }

        [HttpGet("top")]
        public async Task<ActionResult<Game[]>> GetGames(uint pageSize = 10, uint page = 1) {
            try {
                return Ok(await gameService.GetGames(pageSize, page));
            } catch (InvalidDataException ex) {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("search")]
        public async Task<ActionResult<Game[]>> SearchGames(string term, uint pageSize = 10, uint page = 1) {
            try {
                return Ok(await gameService.SearchGames(term, pageSize, page));
            } catch (InvalidDataException ex) {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Game>> GetGameById(ulong id) {
            return Ok(await gameService.GetGameById(id));
        }
    }
}