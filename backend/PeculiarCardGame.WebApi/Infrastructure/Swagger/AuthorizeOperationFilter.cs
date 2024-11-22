using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using PeculiarCardGame.WebApi.Infrastructure.Authentication;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace PeculiarCardGame.WebApi.Infrastructure.Swagger
{
    public class AuthorizeOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var authorizeAttributes = context.MethodInfo.DeclaringType!.GetCustomAttributes(true).Union(context.MethodInfo.GetCustomAttributes(true)).OfType<AuthorizeAttribute>();
            var allowAnonymousAttributes = context.MethodInfo.DeclaringType!.GetCustomAttributes(true).Union(context.MethodInfo.GetCustomAttributes(true)).OfType<AllowAnonymousAttribute>();
            if (!authorizeAttributes.Any() || allowAnonymousAttributes.Any())
            {
                operation.Security.Clear();
                return;
            }

            var attribute = authorizeAttributes.First();
            var id = attribute.AuthenticationSchemes switch
            {
                BasicAuthenticationHandler.SchemeName => BasicAuthenticationHandler.SchemeName,
                BearerTokenAuthenticationHandler.SchemeName => BearerTokenAuthenticationHandler.SchemeName,
                _ => null
            };

            if (id is null)
            {
                operation.Security.Clear();
                return;
            }

            operation.Security = new List<OpenApiSecurityRequirement>
            {
                new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Id = id,
                                Type = ReferenceType.SecurityScheme
                            }
                        },
                        new List<string>()
                    }
                }
            };
        }
    }
}
