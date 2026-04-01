using AiReviewHub.Domain.Abstractions;
using AiReviewHub.Domain.Enums;
using AiReviewHub.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace AiReviewHub.Domain.Entities
{
    public class User
    {
        public Guid Id { get; private set; }
        public Email Email { get; private set; } = null!;
        public PasswordHash PasswordHash { get; private set; } = null!;
        public string FirstName { get; private set; } = string.Empty;
        public string LastName { get; private set; } = string.Empty;
        public Plan Plan { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }

        public ICollection<Project> Projects { get; private set; } = [];

        private User() { }

        public static User Create(string email, string passwordHash, string firstName, string lastName, DateTime now)
        {
            if (string.IsNullOrWhiteSpace(firstName))
                throw new ArgumentException("First name cannot be empty");

            if (string.IsNullOrWhiteSpace(lastName))
                throw new ArgumentException("Last name cannot be empty");

            return new User
            {
                Id = Guid.NewGuid(),
                Email = Email.Create(email),
                PasswordHash = PasswordHash.Create(passwordHash),
                FirstName = firstName.Trim(),
                LastName = lastName.Trim(),
                Plan = Plan.Free,
                CreatedAt = now
            };
        }

        public void UpdatePlan(Plan plan, IDateTimeProvider dateTimeProvider)
        {
            if (Plan == plan)
                throw new InvalidOperationException($"User is already on {plan} plan");

            Plan = plan;
            UpdatedAt = dateTimeProvider.UtcNow;
        }
    }

}
