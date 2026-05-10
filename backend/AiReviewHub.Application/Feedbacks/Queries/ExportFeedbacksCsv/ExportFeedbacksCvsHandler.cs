using AiReviewHub.Application.Abstractions;
using AiReviewHub.Domain.Abstractions;
using AiReviewHub.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace AiReviewHub.Application.Feedbacks.Queries.ExportFeedbacksCsv
{
    public class ExportFeedbacksCsvHandler
        : IRequestHandler<ExportFeedbacksCsvQuery, ExportFeedbacksCsvResult>
    {
        private readonly IAppDbContext _context;
        private readonly ICurrentUserService _currentUser;
        private readonly IDateTimeProvider _dateTimeProvider;

        public ExportFeedbacksCsvHandler(
            IAppDbContext context,
            ICurrentUserService currentUser,
            IDateTimeProvider dateTimeProvider)
        {
            _context = context;
            _currentUser = currentUser;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<ExportFeedbacksCsvResult> Handle(
            ExportFeedbacksCsvQuery request,
            CancellationToken cancellationToken)
        {
            // Vérifie que le projet appartient à l'utilisateur
            var project = await _context.Projects
                .AsNoTracking()
                .FirstOrDefaultAsync(p =>
                    p.Id == request.ProjectId &&
                    p.UserId == _currentUser.UserId,
                    cancellationToken)
                ?? throw new NotFoundException("Project not found");

            // Vérifie le plan — export CSV réservé aux plans Pro+
            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == _currentUser.UserId, cancellationToken)
                ?? throw new NotFoundException("User not found");

            if (user.Plan == Domain.Enums.Plan.Free)
                throw new ForbiddenException(
                    "CSV export is available on Pro and Team plans.");

            // Charge les feedbacks
            var query = _context.Feedbacks
                .AsNoTracking()
                .Where(f => f.ProjectId == request.ProjectId);

            if (request.StatusFilter.HasValue)
                query = query.Where(f => f.Status == request.StatusFilter.Value);

            if (request.CategoryFilter.HasValue)
                query = query.Where(f => f.Category == request.CategoryFilter.Value);

            if (request.PriorityFilter.HasValue)
                query = query.Where(f => f.Priority == request.PriorityFilter.Value);

            var feedbacks = await query
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync(cancellationToken);

            // Génère le CSV
            var csv = BuildCsv(feedbacks, project.Name);

            var fileName = $"feedbacks_{project.Name.ToLower().Replace(" ", "_")}_{_dateTimeProvider.UtcNow:yyyy-MM-dd}.csv";

            return new ExportFeedbacksCsvResult(
                Encoding.UTF8.GetBytes(csv),
                fileName);
        }

        private static string BuildCsv(
            IEnumerable<Domain.Entities.Feedback> feedbacks,
            string projectName)
        {
            var sb = new StringBuilder();

            // BOM UTF-8 pour Excel
            sb.Append('\uFEFF');

            // En-têtes
            sb.AppendLine(
                "ID;" +
                "Contenu;" +
                "Résumé IA;" +
                "Catégorie;" +
                "Priorité;" +
                "Statut;" +
                "Statut Analyse IA;" +
                "Date de création;" +
                "Dernière mise à jour");

            // Lignes
            foreach (var f in feedbacks)
            {
                sb.AppendLine(string.Join(";", new[]
                {
                    EscapeCsv(f.Id.ToString()),
                    EscapeCsv(f.Content.Value),
                    EscapeCsv(f.AiSummary),
                    EscapeCsv(f.Category.ToString()),
                    EscapeCsv(f.Priority.ToString()),
                    EscapeCsv(f.Status.ToString()),
                    EscapeCsv(f.AiAnalysisStatus.ToString()),
                    EscapeCsv(f.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss")),
                    EscapeCsv(f.UpdatedAt?.ToString("yyyy-MM-dd HH:mm:ss") ?? ""),
                }));
            }

            return sb.ToString();
        }

        // Échappe les valeurs CSV — gère les virgules, guillemets et sauts de ligne
        private static string EscapeCsv(string value)
        {
            if (string.IsNullOrEmpty(value)) return "\"\"";

            // Si la valeur contient des guillemets, virgules ou sauts de ligne
            if (value.Contains('"') || value.Contains(',') || value.Contains('\n') || value.Contains('\r'))
            {
                return $"\"{value.Replace("\"", "\"\"")}\"";
            }

            return $"\"{value}\"";
        }
    }
}
