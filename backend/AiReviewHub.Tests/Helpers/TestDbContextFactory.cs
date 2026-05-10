using AiReviewHub.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace AiReviewHub.Tests.Helpers
{
    public static class TestDbContextFactory
    {

        public static AppDbContext CreateInMemory(string? dbName = null)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(dbName ?? Guid.NewGuid().ToString())
                .Options;

            var context = new AppDbContext(options);
            context.Database.EnsureCreated();
            return context;
        }

        // PostgreSQL réel — pour les tests d'intégration
        public static AppDbContext CreatePostgres()
        {
            var connectionString = Environment.GetEnvironmentVariable(
                "ConnectionStrings__DefaultConnection")
                ?? "Host=localhost;Port=5433;Database=aireviewhub_test;Username=postgres;Password=test_password";

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseNpgsql(connectionString)
                .Options;

            var context = new AppDbContext(options);
            context.Database.Migrate(); // applique les migrations
            return context;
        }

        public static AppDbContext Create(string? dbName = null)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(dbName ?? Guid.NewGuid().ToString())
                .Options;

            var context = new AppDbContext(options);
            context.Database.EnsureCreated();
            return context;
        }
    }
}
