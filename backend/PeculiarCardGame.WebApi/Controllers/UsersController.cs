using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PeculiarCardGame.Services;
using PeculiarCardGame.Services.Users;
using PeculiarCardGame.Shared;
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
        [SwaggerOperation("Signs user up.", $"Requires no authentication data to be sent. Username for each user must be unique. If displayed name is null, username will be used. Username and displayed name are trimmed of any leading or trailing whitespace characters.")]
        [SwaggerResponse(201, Description = "User created", Type = typeof(GetUserResponse))]
        [SwaggerResponse(409, "Username already in use", typeof(string))]
        [SwaggerResponse(422, "Endpoint invoked with authentication data", typeof(string))]
        [SwaggerResponse(422, "Username or password null, empty or consisting of only whitespace characters")]
        public ActionResult<GetUserResponse> AddUser(AddUserRequest request)
        {
            if (_requestContext.CallingUser is not null)
                return UnprocessableEntity("This method is only allowed for unauthenticated users.");

            try
            {
                var result = _usersService.AddUser(request.Username, request.DisplayedName, request.Password);
                if (result.IsRight)
                    return CreatedAtAction(nameof(GetUser), new { id = result.Right.Id }, GetUserResponse.FromUser(result.Right));

                return result.Left! switch
                {
                    ErrorType.Conflict => Conflict($"User {request.Username} already exists."),
                    ErrorType.ConstraintsNotMet => UnprocessableEntity($"Username or displayed name too long."),
                    _ => throw new UnreachableException($"result.Left = {result.Left.ToString()}")
                };
            }
            catch (ArgumentNullException ex)
            {
                return UnprocessableEntity(ex.Message);
            }
        }

        [HttpGet("{id:int}", Name = "GetUser")]
        [SwaggerOperation("Gets specified user.", "Doesn't require any authentication data. Only returns basic information about user, not their decks.")]
        [SwaggerResponse(200, "User found", typeof(GetUserResponse))]
        [SwaggerResponse(404, "User not found")]
        public ActionResult<GetUserResponse> GetUser(int id)
        {
            var result = _usersService.GetUser(id);
            if (result.IsRight)
                return Ok(GetUserResponse.FromUser(result.Right!));
            
            return result.Left! switch
            {
                ErrorType.NotFound => NotFound("User not found"),
                _ => throw new UnreachableException($"result.Left = {result.Left.ToString()}")
            };
        }

        [HttpPatch("{id:int}")]
        [Authorize(AuthenticationSchemes = BearerTokenAuthenticationHandler.SchemeName)]
        [SwaggerOperation("Updates specified used.", "Requires valid bearer token authentication data to be sent in 'Authorization' header. Doesn't allow modifying other users. Displayed name is trimmed of any leading or trailing whitespace characters.")]
        [SwaggerResponse(200, "User updated", typeof(GetUserResponse))]
        [SwaggerResponse(401, "Invalid authentication data", typeof(string))]
        [SwaggerResponse(404, "User not found")]
        public ActionResult<GetUserResponse> UpdateUser(int id, UpdateUserRequest request)
        {
            var result = _usersService.UpdateUser(id, request.DisplayedNameUpdate, request.PasswordUpdate);
            if (result.IsRight)
                return Ok(GetUserResponse.FromUser(result.Right!));

            return result.Left! switch
            {
                ErrorType.ConstraintsNotMet => UnprocessableEntity("Displayed name too long."),
                ErrorType.NotFound or ErrorType.Unauthorized => NotFound("User not found."),
                _ => throw new UnreachableException($"result.Left = {result.Left.ToString()}")
            };
        }

        [HttpDelete("{id:int}")]
        [Authorize(AuthenticationSchemes = BearerTokenAuthenticationHandler.SchemeName)]
        [SwaggerOperation("Deletes specified used.", "Requires valid bearer token authentication data to be sent in 'Authorization' header. Doesn't allow deleting other users.")]
        [SwaggerResponse(200, "User deleted")]
        [SwaggerResponse(401, "Invalid authentication data", typeof(string))]
        [SwaggerResponse(404, "User not found")]
        public IActionResult DeleteUser(int id)
        {
            var error = _usersService.DeleteUser(id);
            return error switch
            {
                null => Ok(),
                ErrorType.NotFound or ErrorType.Unauthorized => NotFound("User not found."),
                _ => throw new UnreachableException($"error = {error.ToString()}")
            };
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
