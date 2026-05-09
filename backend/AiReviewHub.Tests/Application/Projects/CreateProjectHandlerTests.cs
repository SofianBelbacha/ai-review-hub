using AiReviewHub.Application.Projects.Commands.CreateProject;
using AiReviewHub.Domain.Enums;
using AiReviewHub.Domain.Exceptions;
using AiReviewHub.Domain.Entities;
using AiReviewHub.Infrastructure.Persistence;
using AiReviewHub.Tests.Helpers;
using AutoMapper;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace AiReviewHub.Tests.Application.Projects
{
    public class CreateProjectHandlerTests : IDisposable
    {
        private readonly AppDbContext _context;
        private readonly FakeDateTimeProvider _clock;
        private readonly FakeCurrentUserService _currentUser;
        private readonly Mock<IMapper> _mapper;
        private readonly CreateProjectHandler _handler;

        public CreateProjectHandlerTests()
        {
            _context = TestDbContextFactory.Create();
            _clock = new FakeDateTimeProvider();
            _currentUser = new FakeCurrentUserService();
            _mapper = new Mock<IMapper>();

            _handler = new CreateProjectHandler(
                _context,
                _clock,
                _currentUser,
                _mapper.Object);
        }

        [Fact]
        public async Task Handle_WithValidCommand_ShouldCreateProject()
        {
            // Arrange
            var user = EntityBuilders.BuildUser();
            _context.Users.Add(user);
            await _context.SaveChangesAsync(TestContext.Current.CancellationToken);

            _currentUser.UserId = user.Id;

            _mapper.Setup(m => m.Map<CreateProjectResult>(It.IsAny<Project>()))
                   .Returns(new CreateProjectResult(
                       Guid.NewGuid(), "Mon Projet", "", "token123", true, _clock.UtcNow));

            var command = new CreateProjectCommand("Mon Projet", "Description");

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            _context.Projects.Should().HaveCount(1);
            _context.Projects.Single().Name.Should().Be("Mon Projet");
            _context.Projects.Single().UserId.Should().Be(user.Id);
        }

        [Fact]
        public async Task Handle_FreePlanWithExistingProject_ShouldThrowForbidden()
        {
            // Arrange
            var user = EntityBuilders.BuildUser(plan: Plan.Free);
            _context.Users.Add(user);

            // Projet existant — Free plan limite à 1
            var existingProject = Project.Create(
                "Projet existant", "", user.Id, _clock.UtcNow);
            _context.Projects.Add(existingProject);
            await _context.SaveChangesAsync(TestContext.Current.CancellationToken);

            _currentUser.UserId = user.Id;

            var command = new CreateProjectCommand("Deuxième Projet", "");

            // Act
            var act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ForbiddenException>()
                     .WithMessage("*Free*");
        }

        [Fact]
        public async Task Handle_ProPlanWithinLimit_ShouldSucceed()
        {
            // Arrange
            var user = EntityBuilders.BuildUser(plan: Plan.Pro);
            _context.Users.Add(user);

            // 9 projets existants — Pro limite à 10
            for (var i = 0; i < 9; i++)
            {
                _context.Projects.Add(Project.Create(
                    $"Projet {i}", "", user.Id, _clock.UtcNow));
            }
            await _context.SaveChangesAsync(TestContext.Current.CancellationToken);

            _currentUser.UserId = user.Id;

            _mapper.Setup(m => m.Map<CreateProjectResult>(It.IsAny<Project>()))
                   .Returns(new CreateProjectResult(
                       Guid.NewGuid(), "10ème Projet", "", "token", true, _clock.UtcNow));

            var command = new CreateProjectCommand("10ème Projet", "");

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            _context.Projects.Should().HaveCount(10);
        }

        [Fact]
        public async Task Handle_ProPlanAtLimit_ShouldThrowForbidden()
        {
            // Arrange
            var user = EntityBuilders.BuildUser(plan: Plan.Pro);
            _context.Users.Add(user);

            // 10 projets — limite Pro atteinte
            for (var i = 0; i < 10; i++)
            {
                _context.Projects.Add(Project.Create(
                    $"Projet {i}", "", user.Id, _clock.UtcNow));
            }
            await _context.SaveChangesAsync(TestContext.Current.CancellationToken);

            _currentUser.UserId = user.Id;

            var command = new CreateProjectCommand("11ème Projet", "");

            // Act
            var act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ForbiddenException>()
                     .WithMessage("*Pro*");
        }

        [Fact]
        public async Task Handle_WithNonExistentUser_ShouldThrowNotFoundException()
        {
            // Arrange — user inexistant en DB
            _currentUser.UserId = Guid.NewGuid();

            var command = new CreateProjectCommand("Mon Projet", "");

            // Act
            var act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task Handle_ShouldGenerateUniquePublicToken()
        {
            // Arrange
            var user = EntityBuilders.BuildUser();
            _context.Users.Add(user);
            await _context.SaveChangesAsync(TestContext.Current.CancellationToken);

            _currentUser.UserId = user.Id;
            _mapper.Setup(m => m.Map<CreateProjectResult>(It.IsAny<Project>()))
                   .Returns((Project p) => new CreateProjectResult(
                       p.Id, p.Name, p.Description, p.PublicToken, p.IsActive, p.CreatedAt));

            // Act — crée deux projets
            await _handler.Handle(
                new CreateProjectCommand("Projet A", ""), CancellationToken.None);

            // Remet un user Free à 0 pour le test
            _context.Projects.RemoveRange(_context.Projects);
            await _context.SaveChangesAsync(TestContext.Current.CancellationToken);

            await _handler.Handle(
                new CreateProjectCommand("Projet B", ""), CancellationToken.None);

            // Assert — les tokens sont différents
            var tokens = _context.Projects.Select(p => p.PublicToken).ToList();
            tokens.Should().OnlyHaveUniqueItems();
        }

        public void Dispose() => _context.Dispose();
    }
}
