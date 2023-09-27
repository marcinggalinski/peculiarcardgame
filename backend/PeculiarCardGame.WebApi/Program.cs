using PeculiarCardGame.Data;
using PeculiarCardGame.Options;
using PeculiarCardGame.Services;
using PeculiarCardGame.Services.DeckManagement;
using PeculiarCardGame.Services.Users;
using PeculiarCardGame.WebApi.Auth;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<SqlServerOptions>(builder.Configuration.GetSection(SqlServerOptions.ConfigurationKey));

builder.Services.AddDbContext<PeculiarCardGameDbContext>();

builder.Services.AddScoped<RequestContext>();
builder.Services.AddScoped<IUsersService, UsersService>();
builder.Services.AddScoped<IDeckManagementService, DeckManagementService>();

builder.Services.AddAuthentication(options =>
{
    options.AddScheme(BasicAuthenticationHandler.SchemeName, builder => builder.HandlerType = typeof(BasicAuthenticationHandler));
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
