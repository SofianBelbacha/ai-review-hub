using AiReviewHub.Api;
using AiReviewHub.Api.Middleware;
using AiReviewHub.Infrastructure.Jobs;
using AiReviewHub.Infrastructure.Persistence;
using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Ajout du service
builder.Services.AddHealthChecks();

builder.Services.AddAppDI(builder.Configuration);

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
            ),
            ClockSkew = TimeSpan.Zero
        };
    });

// Program.cs
builder.Services.AddCors(options =>
{
    options.AddPolicy("Development", policy =>
    {
        policy
            .WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials(); // ← indispensable pour les cookies
    });

    options.AddPolicy("Production", policy =>
    {
        policy
            .WithOrigins("https://ton-domaine.com")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});


builder.Services.AddAuthorization();

var app = builder.Build();

// Appliquer les migrations automatiquement au démarrage
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
}

app.UseCors(app.Environment.IsDevelopment() ? "Development" : "Production");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseHangfireDashboard("/hangfire");
}

//app.UseHttpsRedirection();

app.UseMiddleware<ExceptionMiddleware>();


// Planifie le job toutes les nuits à minuit
RecurringJob.AddOrUpdate<RefreshTokenCleanupJob>(
    "cleanup-refresh-tokens",
    job => job.CleanupExpiredTokens(),
    Cron.Daily);

app.UseAuthentication();

app.UseAuthorization();

// Mapping de l'endpoint
app.MapHealthChecks("/health");

app.MapControllers();

app.Run();
