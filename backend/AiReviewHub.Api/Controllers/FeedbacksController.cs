using AiReviewHub.Application.Feedbacks.Commands.CreateFeedback;
using AiReviewHub.Application.Feedbacks.Commands.UpdateFeedbackStatus;
using AiReviewHub.Application.Feedbacks.Queries.GetFeedbacksByProject;
using AiReviewHub.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AiReviewHub.Api.Controllers
{
    [ApiController]
    [Route("api/projects/{projectId:guid}/feedbacks")]
    [Authorize]
    public class FeedbacksController : ControllerBase
    {
        private readonly IMediator _mediator;

        public FeedbacksController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            Guid projectId,
            [FromQuery] FeedbackStatus? status,
            [FromQuery] FeedbackCategory? category,
            [FromQuery] FeedbackPriority? priority,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(
                new GetFeedbacksByProjectQuery(projectId, status, category, priority),
                cancellationToken);

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            Guid projectId,
            [FromBody] CreateFeedbackRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(
                new CreateFeedbackCommand(request.Content, projectId),
                cancellationToken);

            return Created($"api/projects/{projectId}/feedbacks/{result.Id}", result);
        }

        [HttpPatch("{feedbackId:guid}/status")]
        public async Task<IActionResult> UpdateStatus(
            Guid feedbackId,
            [FromBody] UpdateFeedbackStatusRequest request,
            CancellationToken cancellationToken)
        {
            await _mediator.Send(
                new UpdateFeedbackStatusCommand(feedbackId, request.NewStatus),
                cancellationToken);

            return NoContent();
        }
    }

    // DTOs de requête spécifiques au controller
    public record CreateFeedbackRequest(string Content);
    public record UpdateFeedbackStatusRequest(FeedbackStatus NewStatus);
}
