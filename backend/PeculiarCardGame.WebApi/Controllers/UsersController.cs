using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PeculiarCardGame.Services;
using PeculiarCardGame.Services.Users;
using PeculiarCardGame.WebApi.Infrastructure.Authentication;
using PeculiarCardGame.WebApi.Models.Requests;
using PeculiarCardGame.WebApi.Models.Responses;

namespace PeculiarCardGame.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly RequestContext _requestContext;
        private readonly IUsersService _usersService;

        public UsersController(RequestContext requestContext, IUsersService usersService)
        {
            _requestContext = requestContext;
            _usersService = usersService;
        }

        [HttpPost]
        public ActionResult<GetUserResponse> AddUser(AddUserRequest request)
        {
            if (_requestContext.CallingUser is not null)
                return UnprocessableEntity("This method is only allowed for unauthenticated users.");

            var user = _usersService.AddUser(request.Username, request.DisplayedName, request.Password);
            if (user is null)
                return UnprocessableEntity($"User {request.Username} already exists.");
            return CreatedAtAction(nameof(GetUser), new { username = user.Username }, new GetUserResponse
            {
                Username = request.Username,
                DisplayedName = request.DisplayedName
            });
        }

        [HttpGet("{username}")]
        public ActionResult<GetUserResponse> GetUser(string username)
        {
            var user = _usersService.GetUser(username);
            if (user is null)
                return NotFound();
            return Ok(new GetUserResponse
            {
                Username = user.Username,
                DisplayedName = user.DisplayedName
            });
        }

        [HttpPatch("{username}")]
        [Authorize(AuthenticationSchemes = BearerTokenAuthenticationHandler.SchemeName)]
        public ActionResult<GetUserResponse> UpdateUser(string username, UpdateUserRequest request)
        {
            var user = _usersService.UpdateUser(username, request.DisplayedUsernameUpdate, request.PasswordUpdate);
            if (user is null)
                return NotFound();
            return Ok(new GetUserResponse
            {
                DisplayedName = user.DisplayedName,
                Username = user.Username
            });
        }

        [HttpDelete("{username}")]
        [Authorize(AuthenticationSchemes = BearerTokenAuthenticationHandler.SchemeName)]
        public IActionResult DeleteUser(string username)
        {
            var isDeleted = _usersService.DeleteUser(username);
            return isDeleted ? Ok() : NotFound();
        }

        [HttpPost("signin")]
        [Authorize(AuthenticationSchemes = BasicAuthenticationHandler.SchemeName)]
        public ActionResult<SignInResponse> SignIn()
        {
            return Ok(new SignInResponse
            {
                Token = Request.HttpContext.User.Claims.Single(x => x.Type == "BearerToken").Value
            });
        }
    }
}
