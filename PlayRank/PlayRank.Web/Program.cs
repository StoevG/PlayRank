using PlayRank.Api.Extensions;
using PlayRank.Application.Core.Interfaces.External;
using PlayRank.Application.Core.Services.External;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.RegisterAutoMapper();

builder.Services.AddHttpClient<IFootballTeamService, FootballTeamService>(client =>
{
    client.BaseAddress = new Uri("https://v3.football.api-sports.io/");
});

builder.Services.RegisterRepositories();
builder.Services.RegisterServices();

builder.Services.RegisterDbContext(builder.Configuration);

builder.Services.AddCustomSwaggerExtension();

var app = builder.Build();

app.UseCustomExceptionHandling();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();


app.MapControllers();

app.Run();