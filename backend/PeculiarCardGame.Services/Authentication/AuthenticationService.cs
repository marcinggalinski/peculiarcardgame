using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PeculiarCardGame.Data;
using PeculiarCardGame.Data.Models;
using PeculiarCardGame.Shared.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Web.Helpers;
using Microsoft.EntityFrameworkCore;
using PeculiarCardGame.Shared;

namespace PeculiarCardGame.Services.Authentication
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly TimeSpan _accessTokenLifetime = TimeSpan.FromHours(1);
        private readonly TimeSpan _refreshTokenLifetime = TimeSpan.FromDays(3);

        private readonly BearerTokenAuthenticationSchemeOptions _jwtOptions;
        private readonly PeculiarCardGameDbContext _dbContext;
        private readonly RequestContext _requestContext;

        public AuthenticationService(
            IOptions<BearerTokenAuthenticationSchemeOptions> options,
            PeculiarCardGameDbContext dbContext,
            RequestContext requestContext)
        {
            _jwtOptions = options.Value;
            _dbContext = dbContext;
            _requestContext = requestContext;
        }

        public Either<ErrorType, User> Authenticate(string username, string password)
        {
            ArgumentNullException.ThrowIfNull(username);
            ArgumentNullException.ThrowIfNull(password);

            var user = _dbContext.Users.SingleOrDefault(x => x.Username == username);
            if (user is null)
                return ErrorType.NotFound;

            if (!Crypto.VerifyHashedPassword(user.PasswordHash, password))
                return ErrorType.AuthenticationFailed;

            return user;
        }

        public Either<ErrorType, User> Authenticate(string token)
        {
            ArgumentNullException.ThrowIfNull(token);

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

        public (string AccessToken, string RefreshToken) GenerateTokens(string audience)
        {
            ArgumentNullException.ThrowIfNull(audience);
            if (_requestContext.CallingUser is null)
                throw new InvalidOperationException($"{nameof(GenerateTokens)} can only be called by an authenticated user.");

            var user = _requestContext.CallingUser;
            var accessToken = GenerateAccessToken(audience, user.Id, user.Username, user.DisplayedName);
            var refreshToken = GenerateRefreshToken(_requestContext.CallingUser.Id);

            return (accessToken, refreshToken);
        }

        public Either<ErrorType, (string AccessToken, string RefreshToken)> RefreshTokens(string refreshToken, string audience)
        {
            var dbToken = _dbContext.TokenInfos
                .Include(tokenInfo => tokenInfo.User)
                .SingleOrDefault(x => x.Token == refreshToken);
            if (dbToken is null || dbToken.IsRevoked || dbToken.ExpirationDateUtc < DateTime.UtcNow)
                return ErrorType.AuthenticationFailed;

            dbToken.IsRevoked = true;
            _dbContext.TokenInfos.Update(dbToken);
            _dbContext.SaveChanges();

            _requestContext.SetOnce(dbToken.User);
            return GenerateTokens(audience);
        }

        public void RevokeRefreshToken(string refreshToken)
        {
            var dbToken = _dbContext.TokenInfos
                .Include(tokenInfo => tokenInfo.User)
                .SingleOrDefault(x => x.Token == refreshToken);
            if (dbToken is null || dbToken.IsRevoked || dbToken.ExpirationDateUtc < DateTime.UtcNow)
                return;

            dbToken.IsRevoked = true;
            _dbContext.TokenInfos.Update(dbToken);
            _dbContext.SaveChanges();
        }

        private string GenerateAccessToken(string audience, int userId, string username, string displayedName)
        {
            if (_requestContext.CallingUser is null)
                throw new InvalidOperationException($"{nameof(GenerateAccessToken)} can only be called by an authenticated user.");
            
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _jwtOptions.ClaimsIssuer,
                Audience = audience,
                Subject = new ClaimsIdentity(new List<Claim>
                {
                    new Claim("id", userId.ToString()),
                    new Claim("name", username),
                    new Claim("nickname", displayedName)
                }),
                Expires = DateTime.UtcNow.Add(_accessTokenLifetime),
                SigningCredentials = credentials
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));
            return token;
        }
        
        private string GenerateRefreshToken(int userId)
        {
            var token = Guid.NewGuid().ToString("N");
            _dbContext.TokenInfos.Add(new TokenInfo
            {
                Token = token,
                ExpirationDateUtc = DateTime.UtcNow.Add(_refreshTokenLifetime),
                UserId = userId
            });
            _dbContext.SaveChanges();

            return token;
        }
    }
}
