using PlayRank.Api.Extensions;
using PlayRank.Application.Core.Interfaces.External;
using PlayRank.Application.Core.Services.External;
using PlayRank.Domain.Constants;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.RegisterAutoMapper();

builder.Services.AddHttpClient<IFootballTeamService, FootballTeamService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration[ExternalFootballConstants.BaseUrlConfigKey]!);
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