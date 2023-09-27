using PeculiarCardGame.Data;
using PeculiarCardGame.Options;
using PeculiarCardGame.Services.DeckManagement;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<SqlServerOptions>(builder.Configuration.GetSection(SqlServerOptions.ConfigurationKey));

builder.Services.AddDbContext<PeculiarCardGameDbContext>();

builder.Services.AddScoped<IDeckManagementService, DeckManagementService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();
app.MapControllers();

app.Run();
