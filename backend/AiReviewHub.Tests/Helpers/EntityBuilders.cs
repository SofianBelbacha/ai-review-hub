using AiReviewHub.Domain.Entities;
using AiReviewHub.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace AiReviewHub.Tests.Helpers
{
    /// <summary>
    /// Builders pour créer des entités de test avec des valeurs cohérentes
    /// </summary>
    public static class EntityBuilders
    {
        private static readonly FakeDateTimeProvider Clock = new();

        public static User BuildUser(
            string email = "test@example.com",
            string password = "Password123!",
            string firstName = "Jean",
            string lastName = "Dupont",
            Plan plan = Plan.Free)
        {
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
            var user = User.Create(email, passwordHash, firstName, lastName, Clock.UtcNow);

            if (plan != Plan.Free)
                user.UpdatePlan(plan, Clock);

            return user;
        }

        public static Project BuildProject(
            Guid? userId = null,
            string name = "Projet Test",
            string description = "Description test")
        {
            return Project.Create(
                name,
                description,
                userId ?? Guid.NewGuid(),
                Clock.UtcNow);
        }

        public static Feedback BuildFeedback(
            Guid? projectId = null,
            string content = "Ceci est un feedback de test avec assez de contenu.")
        {
            return Feedback.Create(
                content,
                projectId ?? Guid.NewGuid(),
                Clock.UtcNow);
        }
    }
}
