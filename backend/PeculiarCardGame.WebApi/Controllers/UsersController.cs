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
            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, GetUserResponse.FromUser(user));
        }

        [HttpGet("{id}", Name = "GetUser")]
        public ActionResult<GetUserResponse> GetUser(int id)
        {
            var user = _usersService.GetUser(id);
            if (user is null)
                return NotFound();
            return Ok(GetUserResponse.FromUser(user));
        }

        [HttpPatch("{id}")]
        [Authorize(AuthenticationSchemes = BearerTokenAuthenticationHandler.SchemeName)]
        public ActionResult<GetUserResponse> UpdateUser(int id, UpdateUserRequest request)
        {
            var user = _usersService.UpdateUser(id, request.DisplayedNameUpdate, request.PasswordUpdate);
            if (user is null)
                return NotFound();
            return Ok(GetUserResponse.FromUser(user));
        }

        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = BearerTokenAuthenticationHandler.SchemeName)]
        public IActionResult DeleteUser(int id)
        {
            var isDeleted = _usersService.DeleteUser(id);
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
