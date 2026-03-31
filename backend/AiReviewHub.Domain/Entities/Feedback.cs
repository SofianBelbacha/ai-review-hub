using AiReviewHub.Domain.Enums;

namespace AiReviewHub.Domain.Entities
{
    public class Feedback
    {
        public Guid Id { get; private set; }
        public string Content { get; private set; } = string.Empty;

        // Champs enrichis par OpenAI
        public FeedbackCategory Category { get; private set; }
        public FeedbackPriority Priority { get; private set; }
        public string AiSummary { get; private set; } = string.Empty;

        // Workflow kanban
        public FeedbackStatus Status { get; private set; }

        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }

        // Relations
        public Guid ProjectId { get; private set; }
        public Project Project { get; private set; } = null!;

        private Feedback() { }

        public static Feedback Create(string content, Guid projectId)
        {
            return new Feedback
            {
                Id = Guid.NewGuid(),
                Content = content,
                Category = FeedbackCategory.Uncategorized,
                Priority = FeedbackPriority.Normal,
                Status = FeedbackStatus.Todo,
                AiSummary = string.Empty,
                ProjectId = projectId,
                CreatedAt = DateTime.UtcNow
            };
        }

        public void EnrichWithAi(FeedbackCategory category, FeedbackPriority priority, string summary)
        {
            Category = category;
            Priority = priority;
            AiSummary = summary;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateStatus(FeedbackStatus status)
        {
            Status = status;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}