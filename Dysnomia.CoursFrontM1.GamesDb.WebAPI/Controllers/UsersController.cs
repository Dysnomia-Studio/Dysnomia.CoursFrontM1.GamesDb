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
        public async Task<ActionResult<string>> AuthenticateUser(UserAuthenticationRequest authenticationRequest) {
            var token = await usersService.AuthenticateUser(authenticationRequest.Username, authenticationRequest.Password);

            if (token == null) {
                return Unauthorized();
            }

            return Ok(token);
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult<string>> RegisterUser(UserRegistrationRequest registrationRequest) {
            try {
                var token = await usersService.Register(registrationRequest);

                return Ok(token);
            } catch (InvalidDataException ex) {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("renewToken")]
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
        public async Task<ActionResult<UserDto>> GetUserFavoriteGames() {
            var user = await usersService.GetByUsername(HttpContext?.User?.Identity?.Name);

            if (user == default) {
                return NoContent();
            }

            return Ok(user);
        }


    }
}