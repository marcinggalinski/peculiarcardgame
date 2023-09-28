using Microsoft.OpenApi.Models;
using PeculiarCardGame.Data;
using PeculiarCardGame.Options;
using PeculiarCardGame.Services;
using PeculiarCardGame.Services.Authentication;
using PeculiarCardGame.Services.DeckManagement;
using PeculiarCardGame.Services.Users;
using PeculiarCardGame.WebApi.Infrastructure.Auth;
using PeculiarCardGame.WebApi.Infrastructure.Swagger;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    foreach (var scheme in new[] { BasicAuthenticationHandler.SchemeName, BearerTokenAuthenticationHandler.SchemeName })
    {
        options.AddSecurityDefinition(scheme, new OpenApiSecurityScheme
        {
            Name = scheme,
            Scheme = scheme,
            Type = SecuritySchemeType.Http,
            Reference = new OpenApiReference
            {
                Id = scheme,
                Type = ReferenceType.SecurityScheme
            }
        });
    }

    options.OperationFilter<AuthorizeOperationFilter>();
});

builder.Services.Configure<SqlServerOptions>(builder.Configuration.GetSection(SqlServerOptions.ConfigurationKey));
builder.Services.Configure<BasicAuthenticationSchemeOptions>(builder.Configuration.GetSection(BasicAuthenticationSchemeOptions.ConfigurationKey));
builder.Services.Configure<BearerTokenAuthenticationSchemeOptions>(builder.Configuration.GetSection(BearerTokenAuthenticationSchemeOptions.ConfigurationKey));

builder.Services.AddDbContext<PeculiarCardGameDbContext>();

builder.Services.AddScoped<RequestContext>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IUsersService, UsersService>();
builder.Services.AddScoped<IDeckManagementService, DeckManagementService>();

builder.Services.AddAuthentication(options =>
{
    options.AddScheme(BasicAuthenticationHandler.SchemeName, builder => builder.HandlerType = typeof(BasicAuthenticationHandler));
    options.AddScheme(BearerTokenAuthenticationHandler.SchemeName, builder => builder.HandlerType = typeof(BearerTokenAuthenticationHandler));
    options.DefaultAuthenticateScheme = BearerTokenAuthenticationHandler.SchemeName;
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
