namespace AiReviewHub.Domain.Entities
{
    public class Project
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;
        public string PublicToken { get; private set; } = string.Empty;
        public bool IsActive { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }

        // Relations
        public Guid UserId { get; private set; }
        public User User { get; private set; } = null!;
        public ICollection<Feedback> Feedbacks { get; private set; } = [];

        private Project() { }

        public static Project Create(string name, string description, Guid userId)
        {
            return new Project
            {
                Id = Guid.NewGuid(),
                Name = name,
                Description = description,
                PublicToken = Guid.NewGuid().ToString("N"), // token public pour le widget
                IsActive = true,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };
        }

        public void Update(string name, string description)
        {
            Name = name;
            Description = description;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}