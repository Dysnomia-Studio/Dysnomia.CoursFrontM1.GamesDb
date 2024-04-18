using Dysnomia.CoursFrontM1.GamesDb.Business.Interfaces;
using Dysnomia.CoursFrontM1.GamesDb.Common;
using Dysnomia.CoursFrontM1.GamesDb.Common.Dto;
using Dysnomia.CoursFrontM1.GamesDb.Common.Requests;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dysnomia.CoursFrontM1.GamesDb.WebAPI.Controllers {
	[Authorize(Policy = AuthPolicies.JWT_POLICY)]
	[ApiController]
	[Route("api/[controller]")]
	public class UsersController : ControllerBase {
		private readonly IUserService usersService;
		public UsersController(IUserService usersService) {
			this.usersService = usersService;
		}

		[AllowAnonymous]
		[HttpPost("auth")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public async Task<ActionResult<string>> AuthenticateUser(UserAuthenticationRequest authenticationRequest) {
			var token = await usersService.AuthenticateUser(authenticationRequest.Username, authenticationRequest.Password);

			if (token == null) {
				return Unauthorized();
			}

			return Ok(token);
		}

		[AllowAnonymous]
		[HttpPost("register")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<ActionResult<string>> RegisterUser(UserRegistrationRequest registrationRequest) {
			try {
				var token = await usersService.Register(registrationRequest);

				return Ok(token);
			} catch (InvalidDataException ex) {
				return BadRequest(ex.Message);
			}
		}

		[HttpGet("renewToken")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public ActionResult<string> RenewToken() {
			var name = HttpContext?.User?.Identity?.Name;
			if (name == null) {
				return NoContent();
			}
			var token = usersService.RenewToken(name);

			if (token == default) {
				return NoContent();
			}

			return Ok(token);
		}

		[HttpGet("me")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public async Task<ActionResult<UserDto>> GetUserFavoriteGames() {
			var user = await usersService.GetByUsername(HttpContext?.User?.Identity?.Name);

			if (user == default) {
				return NoContent();
			}

			return Ok(user);
		}

		[HttpPost("favorites/add/{gameId}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status409Conflict)]
		public async Task<ActionResult> AddGameToFavorites(ulong gameId) {
			try {
				await usersService.AddGameToFavorites(HttpContext?.User?.Identity?.Name, gameId);
				return NoContent();
			} catch (InvalidDataException) {
				return Conflict();
			}
		}

		[HttpDelete("favorites/remove/{gameId}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public async Task<ActionResult> RemoveGameFromFavorites(ulong gameId) {
			await usersService.RemoveGameFromFavorites(HttpContext?.User?.Identity?.Name, gameId);
			return NoContent();
		}
	}
}
