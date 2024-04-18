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

		[HttpGet("{id}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public async Task<ActionResult<Game>> GetGameById(ulong id) {
			return Ok(await gameService.GetGameById(id));
		}
	}
}
