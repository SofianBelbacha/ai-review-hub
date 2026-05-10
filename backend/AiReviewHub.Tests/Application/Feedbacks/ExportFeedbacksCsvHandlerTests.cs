using AiReviewHub.Application.Abstractions;
using AiReviewHub.Application.Feedbacks.Commands.CreateFeedback;
using AiReviewHub.Application.Feedbacks.Queries.ExportFeedbacksCsv;
using AiReviewHub.Domain.Entities;
using AiReviewHub.Domain.Enums;
using AiReviewHub.Domain.Exceptions;
using AiReviewHub.Infrastructure.Persistence;
using AiReviewHub.Tests.Helpers;
using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;

namespace AiReviewHub.Tests.Application.Feedbacks
{
    public class ExportFeedbacksCsvHandlerTests : IDisposable
    {
        private readonly AppDbContext _context;
        private readonly FakeDateTimeProvider _clock;
        private readonly FakeCurrentUserService _currentUser;
        private readonly ExportFeedbacksCsvHandler _handler;

        public ExportFeedbacksCsvHandlerTests()
        {
            _context = TestDbContextFactory.Create();
            _clock = new FakeDateTimeProvider();
            _currentUser = new FakeCurrentUserService();
            _handler = new ExportFeedbacksCsvHandler(_context, _currentUser, _clock);
        }

        [Fact]
        public async Task Handle_FreePlan_ShouldThrowForbidden()
        {
            var user = EntityBuilders.BuildUser(plan: Plan.Free);
            var project = EntityBuilders.BuildProject(user.Id);
            _context.Users.Add(user);
            _context.Projects.Add(project);
            await _context.SaveChangesAsync(TestContext.Current.CancellationToken);

            _currentUser.UserId = user.Id;

            var query = new ExportFeedbacksCsvQuery(project.Id);
            var act = async () => await _handler.Handle(query, CancellationToken.None);

            await act.Should().ThrowAsync<ForbiddenException>()
                     .WithMessage("*Pro*");
        }

        [Fact]
        public async Task Handle_ProPlan_ShouldReturnCsvBytes()
        {
            var user = EntityBuilders.BuildUser(plan: Plan.Pro);
            var project = EntityBuilders.BuildProject(user.Id);
            var feedback = EntityBuilders.BuildFeedback(project.Id);
            _context.Users.Add(user);
            _context.Projects.Add(project);
            _context.Feedbacks.Add(feedback);
            await _context.SaveChangesAsync(TestContext.Current.CancellationToken);

            _currentUser.UserId = user.Id;

            var result = await _handler.Handle(
                new ExportFeedbacksCsvQuery(project.Id),
                CancellationToken.None);

            result.Content.Should().NotBeEmpty();
            result.FileName.Should().EndWith(".csv");

            // Vérifie que le CSV contient les en-têtes
            var csv = Encoding.UTF8.GetString(result.Content);
            csv.Should().Contain("Contenu");
            csv.Should().Contain("Catégorie");
            csv.Should().Contain("Priorité");
        }

        [Fact]
        public async Task Handle_ShouldEscapeCommasInContent()
        {
            var user = EntityBuilders.BuildUser(plan: Plan.Pro);
            var project = EntityBuilders.BuildProject(user.Id);
            var feedback = Feedback.Create(
                "Contenu avec, virgule et \"guillemets\"",
                project.Id,
                _clock.UtcNow);

            _context.Users.Add(user);
            _context.Projects.Add(project);
            _context.Feedbacks.Add(feedback);
            await _context.SaveChangesAsync(TestContext.Current.CancellationToken);

            _currentUser.UserId = user.Id;

            var result = await _handler.Handle(
                new ExportFeedbacksCsvQuery(project.Id),
                CancellationToken.None);

            var csv = Encoding.UTF8.GetString(result.Content);

            // Les guillemets doivent être doublés dans le CSV
            csv.Should().Contain("\"\"guillemets\"\"");
        }

        public void Dispose() => _context.Dispose();

    }
}
