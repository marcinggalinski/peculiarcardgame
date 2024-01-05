using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using PeculiarCardGame.Data;
using PeculiarCardGame.Options;
using PeculiarCardGame.Services;
using PeculiarCardGame.Services.Authentication;
using PeculiarCardGame.Services.DeckManagement;
using PeculiarCardGame.Services.Users;
using PeculiarCardGame.WebApi.Infrastructure.Authentication;
using PeculiarCardGame.WebApi.Infrastructure.Swagger;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRouting(options =>
{
    options.LowercaseUrls = true;
    options.LowercaseQueryStrings = true;
});
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

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

builder.Services.Configure<DbOptions>(builder.Configuration.GetSection(DbOptions.ConfigurationKey));
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
app.UseSwaggerUI(options =>
{
    options.EnablePersistAuthorization();
});

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

if (!app.Environment.IsEnvironment("ApiTests"))
{
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<PeculiarCardGameDbContext>();
        if (dbContext.Database.GetPendingMigrations().Any())
            dbContext.Database.Migrate();
    }
}

app.Run();
