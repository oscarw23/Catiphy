using Catiphy.Application.Interfaces;
using Catiphy.Infrastructure.Clients;
using Catiphy.Infrastructure.Repositories;
using Catiphy.Infrastructure.Sql;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// ===== REGISTRO DE SERVICIOS (antes de Build) =====
builder.Services.AddControllers();               // Controllers
builder.Services.AddEndpointsApiExplorer();      // Swagger minimal
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Catiphy API", Version = "v1" });
});
builder.Services.AddHttpClient<ICatFactsClient, CatFactsClient>(c =>
{
    c.BaseAddress = new Uri("https://catfact.ninja/facts");
    c.Timeout = TimeSpan.FromSeconds(10);
});
builder.Services.AddHttpClient<IGiphyClient, GiphyClient>(c =>
{
    c.BaseAddress = new Uri("https://api.giphy.com/");
    c.Timeout = TimeSpan.FromSeconds(10);
});


// DI de infraestructura
builder.Services.AddSingleton<IDbConnectionFactory, SqlConnectionFactory>();
builder.Services.AddSingleton<ISearchHistoryRepository, SearchHistoryRepository>();

// ===== CONSTRUIR APP =====
var app = builder.Build();

// ===== MIDDLEWARES / PIPELINE =====
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseStaticFiles();

app.UseHttpsRedirection();

// Ruteo de controllers
app.MapControllers();

// (Opcional) endpoint de salud de DB

app.Run();
