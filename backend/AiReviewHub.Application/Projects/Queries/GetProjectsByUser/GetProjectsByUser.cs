using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AiReviewHub.Application.Projects.Queries.GetProjectsByUser
{
    public record GetProjectsByUserQuery : IRequest<GetProjectsByUserResult>;

    public record GetProjectsByUserResult(
        IReadOnlyList<ProjectDto> Projects,
        int TotalCount
    );

    public record ProjectDto(
        Guid Id,
        string Name,
        string Description,
        string PublicToken,
        bool IsActive,
        int FeedbackCount,
        DateTime CreatedAt
    );
}
