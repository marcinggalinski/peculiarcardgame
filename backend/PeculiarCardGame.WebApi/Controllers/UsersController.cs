using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PeculiarCardGame.Services;
using PeculiarCardGame.Services.Users;
using PeculiarCardGame.WebApi.Infrastructure.Authentication;
using PeculiarCardGame.WebApi.Models.Requests;
using PeculiarCardGame.WebApi.Models.Responses;
using Swashbuckle.AspNetCore.Annotations;

namespace PeculiarCardGame.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [SwaggerTag("Exposes endpoints for user-related operations.")]
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
        [SwaggerOperation("Signs user up.", "Requires no authentication data to be sent. Username for each user must be unique. If displayed name is null, username will be used.")]
        [SwaggerResponse(201, Description = "User created", Type = typeof(GetUserResponse))]
        [SwaggerResponse(409, "Username already in use", typeof(string))]
        [SwaggerResponse(422, "Enpoint invoked with authentication data", typeof(string))]
        public ActionResult<GetUserResponse> AddUser(AddUserRequest request)
        {
            if (_requestContext.CallingUser is not null)
                return UnprocessableEntity("This method is only allowed for unauthenticated users.");

            var user = _usersService.AddUser(request.Username, request.DisplayedName, request.Password);
            if (user is null)
                return Conflict($"User {request.Username} already exists.");
            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, GetUserResponse.FromUser(user));
        }

        [HttpGet("{id}", Name = "GetUser")]
        [SwaggerOperation("Gets specified user.", "Doesn't require any authentication data. Only returns basic information about user, not their decks.")]
        [SwaggerResponse(200, "User found", typeof(GetUserResponse))]
        [SwaggerResponse(404, "User not found")]
        public ActionResult<GetUserResponse> GetUser(int id)
        {
            var user = _usersService.GetUser(id);
            if (user is null)
                return NotFound();
            return Ok(GetUserResponse.FromUser(user));
        }

        [HttpPatch("{id}")]
        [Authorize(AuthenticationSchemes = BearerTokenAuthenticationHandler.SchemeName)]
        [SwaggerOperation("Updates specified used.", "Requires valid bearer token authentication data to be sent in 'Authorization' header. Doesn't allow modyfying other users.")]
        [SwaggerResponse(200, "User updated", typeof(GetUserResponse))]
        [SwaggerResponse(401, "Invalid authentication data", typeof(string))]
        [SwaggerResponse(404, "User not found")]
        public ActionResult<GetUserResponse> UpdateUser(int id, UpdateUserRequest request)
        {
            var user = _usersService.UpdateUser(id, request.DisplayedNameUpdate, request.PasswordUpdate);
            if (user is null)
                return NotFound();
            return Ok(GetUserResponse.FromUser(user));
        }

        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = BearerTokenAuthenticationHandler.SchemeName)]
        [SwaggerOperation("Deletes specified used.", "Requires valid bearer token authentication data to be sent in 'Authorization' header. Doesn't allow deleting other users.")]
        [SwaggerResponse(200, "User deleted", typeof(GetUserResponse))]
        [SwaggerResponse(401, "Invalid authentication data", typeof(string))]
        [SwaggerResponse(404, "User not found")]
        public IActionResult DeleteUser(int id)
        {
            var isDeleted = _usersService.DeleteUser(id);
            return isDeleted ? Ok() : NotFound();
        }

        [HttpPost("signin")]
        [Authorize(AuthenticationSchemes = BasicAuthenticationHandler.SchemeName)]
        [SwaggerOperation("Signs user in.", "Requires valid basic authentication data to be sent in 'Authorization' header. Returns bearer token to be used to authenticate in other endpoints.")]
        [SwaggerResponse(200, "Signed in", typeof(SignInResponse))]
        [SwaggerResponse(401, "Invalid authentication data", typeof(string))]
        public ActionResult<SignInResponse> SignIn()
        {
            return Ok(new SignInResponse
            {
                Token = Request.HttpContext.User.Claims.Single(x => x.Type == "BearerToken").Value
            });
        }
    }
}
