using AiReviewHub.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace AiReviewHub.Domain.Entities
{
    public class User
    {
        public Guid Id { get; private set; }
        public string Email { get; private set; } = string.Empty;
        public string PasswordHash { get; private set; } = string.Empty;
        public string FirstName { get; private set; } = string.Empty;
        public string LastName { get; private set; } = string.Empty;
        public Plan Plan { get; private set; } = Plan.Free;
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }

        // Navigation
        public ICollection<Project> Projects { get; private set; } = [];

        private User() { } // Requis par EF Core

        public static User Create(string email, string passwordHash, string firstName, string lastName)
        {
            return new User
            {
                Id = Guid.NewGuid(),
                Email = email.ToLowerInvariant(),
                PasswordHash = passwordHash,
                FirstName = firstName,
                LastName = lastName,
                Plan = Plan.Free,
                CreatedAt = DateTime.UtcNow
            };
        }

        public void UpdatePlan(Plan plan)
        {
            Plan = plan;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
