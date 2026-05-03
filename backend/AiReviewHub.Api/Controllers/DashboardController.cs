using AiReviewHub.Application.Dashboard.Queries.GetDashboard;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AiReviewHub.Api.Controllers
{
    [ApiController]
    [Route("api/dashboard")]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DashboardController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> Get(
            [FromQuery] Guid? projectId,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(
                new GetDashboardQuery(projectId),
                cancellationToken);

            return Ok(result);
        }
    }
}
