﻿using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PeculiarCardGame.Data;
using PeculiarCardGame.Data.Models;
using PeculiarCardGame.Shared.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Web.Helpers;
using PeculiarCardGame.Shared;

namespace PeculiarCardGame.Services.Authentication
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly TimeSpan _tokenLifetime = TimeSpan.FromHours(1);

        private readonly BearerTokenAuthenticationSchemeOptions _jwtOptions;
        private readonly PeculiarCardGameDbContext _dbContext;
        private readonly RequestContext _requestContext;

        public AuthenticationService(IOptions<BearerTokenAuthenticationSchemeOptions> options, PeculiarCardGameDbContext dbContext, RequestContext requestContext)
        {
            _jwtOptions = options.Value;
            _dbContext = dbContext;
            _requestContext = requestContext;
        }

        public Either<ErrorType, User> Authenticate(string username, string password)
        {
            if (username is null)
                throw new ArgumentNullException(nameof(username));
            if (password is null)
                throw new ArgumentNullException(nameof(password));

            var user = _dbContext.Users.SingleOrDefault(x => x.Username == username);
            if (user is null)
                return ErrorType.NotFound;

            if (!Crypto.VerifyHashedPassword(user.PasswordHash, password))
                return ErrorType.AuthenticationFailed;

            return user;
        }

        public Either<ErrorType, User> Authenticate(string token)
        {
            if (token is null)
                throw new ArgumentNullException(nameof(token));

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key));

            string username;
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    IssuerSigningKey = key,
                    ValidateIssuer = true,
                    ValidateIssuerSigningKey = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidIssuer = _jwtOptions.ClaimsIssuer,
                    ValidAudiences = _jwtOptions.Audiences
                }, out var validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                username = jwtToken.Claims.Single(x => x.Type == "name").Value;
            }
            catch
            {
                return ErrorType.AuthenticationFailed;
            }

            var user = _dbContext.Users.SingleOrDefault(x => x.Username == username);
            if (user is null)
                return ErrorType.NotFound;

            return user;
        }

        public string GenerateBearerToken(string audience)
        {
            if (audience is null)
                throw new ArgumentNullException(nameof(audience));
            if (_requestContext.CallingUser is null)
                throw new InvalidOperationException($"{nameof(GenerateBearerToken)} can only be called by an authenticated user.");

            var claims = new List<Claim>
            {
                new Claim("id", _requestContext.CallingUser.Id.ToString()),
                new Claim("name", _requestContext.CallingUser.Username),
                new Claim("nickname", _requestContext.CallingUser.DisplayedName)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _jwtOptions.ClaimsIssuer,
                Audience = audience,
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.Add(_tokenLifetime),
                SigningCredentials = credentials
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
