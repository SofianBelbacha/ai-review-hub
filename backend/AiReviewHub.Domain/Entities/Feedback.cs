using AiReviewHub.Domain.Abstractions;
using AiReviewHub.Domain.Enums;
using AiReviewHub.Domain.ValueObjects;

namespace AiReviewHub.Domain.Entities
{
    public class Feedback
    {
        // Transitions d'état autorisées
        private static readonly Dictionary<FeedbackStatus, FeedbackStatus[]> AllowedTransitions = new()
        {
            { FeedbackStatus.Todo,       [FeedbackStatus.InProgress] },
            { FeedbackStatus.InProgress, [FeedbackStatus.Todo, FeedbackStatus.Done] },
            { FeedbackStatus.Done,       [FeedbackStatus.InProgress] }
        };

        public Guid Id { get; private set; }
        public FeedbackContent Content { get; private set; } = null!;
        public FeedbackCategory Category { get; private set; }
        public FeedbackPriority Priority { get; private set; }
        public string AiSummary { get; private set; } = string.Empty;
        public FeedbackStatus Status { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }

        public Guid ProjectId { get; private set; }
        public Project Project { get; private set; } = null!;

        private Feedback() { }

        public static Feedback Create(string content, Guid projectId, IDateTimeProvider dateTimeProvider)
        {
            if (projectId == Guid.Empty)
                throw new ArgumentException("ProjectId cannot be empty");

            return new Feedback
            {
                Id = Guid.NewGuid(),
                Content = FeedbackContent.Create(content),
                Category = FeedbackCategory.Uncategorized,
                Priority = FeedbackPriority.Normal,
                Status = FeedbackStatus.Todo,
                AiSummary = string.Empty,
                ProjectId = projectId,
                CreatedAt = dateTimeProvider.UtcNow
            };
        }

        public void EnrichWithAi(FeedbackCategory category, FeedbackPriority priority, string summary, IDateTimeProvider dateTimeProvider)
        {
            if (string.IsNullOrWhiteSpace(summary))
                throw new ArgumentException("AI summary cannot be empty");

            Category = category;
            Priority = priority;
            AiSummary = summary.Trim();
            UpdatedAt = dateTimeProvider.UtcNow;
        }

        public void UpdateStatus(FeedbackStatus newStatus, IDateTimeProvider dateTimeProvider)
        {
            if (Status == newStatus)
                throw new InvalidOperationException($"Feedback is already in {newStatus} status");

            if (!AllowedTransitions[Status].Contains(newStatus))
                throw new InvalidOperationException(
                    $"Cannot transition from {Status} to {newStatus}");

            Status = newStatus;
            UpdatedAt = dateTimeProvider.UtcNow;
        }
    }
}