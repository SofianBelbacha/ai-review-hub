using AiReviewHub.Application.Abstractions;
using AiReviewHub.Domain.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenAI.Chat;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace AiReviewHub.Infrastructure.Services
{
    public class AiAnalysisService : IAiAnalysisService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<AiAnalysisService> _logger;

        public AiAnalysisService(
            IConfiguration configuration,
            ILogger<AiAnalysisService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<AiAnalysisResult> AnalyzeAsync(
            string content,
            CancellationToken cancellationToken = default)
        {
            var apiKey = _configuration["OpenAI:ApiKey"]!;
            var model = _configuration["OpenAI:Model"] ?? "gpt-4.1-mini";
            var maxTokens = _configuration.GetValue<int>("OpenAI:MaxTokens", 500);

            var client = new ChatClient(model, apiKey);

            var prompt = BuildPrompt(content);

            var response = await client.CompleteChatAsync(
                [
                    ChatMessage.CreateSystemMessage(GetSystemPrompt()),
                ChatMessage.CreateUserMessage(prompt)
                ],
                new ChatCompletionOptions
                {
                    MaxOutputTokenCount = maxTokens,
                    Temperature = 0.2f, // réponses cohérentes
                },
                cancellationToken
            );

            var json = response.Value.Content[0].Text;
            return ParseResponse(json);
        }

        private static string GetSystemPrompt() => """
        Tu es un assistant spécialisé dans l'analyse de feedbacks clients pour des équipes de développement.
        Tu dois analyser chaque feedback et retourner UNIQUEMENT un JSON valide sans markdown, sans explication.
        Réponds toujours en français pour le résumé.
        """;

        private static string BuildPrompt(string content) => $$"""
        Analyse ce feedback client et retourne un JSON avec exactement cette structure :
        {
    
          "category": "Bug" | "FeatureRequest" | "Question" | "Uncategorized",
          "priority": "Low" | "Normal" | "High" | "Critical",
          "summary": "résumé en une phrase claire et concise (max 120 caractères)"
        }

        Règles de priorité :
        - Critical : bloquant, urgent, "ne fonctionne pas du tout", "impossible d'utiliser"
        - High : problème important, sentiment négatif fort, impact majeur
        - Normal : demande standard, problème mineur
        - Low : suggestion, amélioration cosmétique, question simple

        Feedback à analyser :
        "{content}"
        """;

        private AiAnalysisResult ParseResponse(string json)
        {
            try
            {
                // Nettoie le JSON si OpenAI ajoute des backticks
                json = json
                    .Replace("```json", "")
                    .Replace("```", "")
                    .Trim();

                var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                var category = Enum.TryParse<FeedbackCategory>(
                    root.GetProperty("category").GetString(),
                    out var cat) ? cat : FeedbackCategory.Uncategorized;

                var priority = Enum.TryParse<FeedbackPriority>(
                    root.GetProperty("priority").GetString(),
                    out var pri) ? pri : FeedbackPriority.Normal;

                var summary = root.GetProperty("summary").GetString()
                    ?? "Analyse indisponible";

                return new AiAnalysisResult(category, priority, summary);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to parse OpenAI response: {Json}", json);
                throw new InvalidOperationException(
                    $"Failed to parse AI response: {ex.Message}");
            }
        }
    }
}
