using AiReviewHub.Api;
using AiReviewHub.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddAppDI(builder.Configuration);

// Ajout du service
builder.Services.AddHealthChecks();

var app = builder.Build();

// Appliquer les migrations automatiquement au démarrage
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Mapping de l'endpoint
app.MapHealthChecks("/health");

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
