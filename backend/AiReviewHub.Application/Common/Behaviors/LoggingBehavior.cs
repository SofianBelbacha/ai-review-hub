using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace AiReviewHub.Application.Common.Behaviors
{
    public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
    {
        private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

        public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var requestName = typeof(TRequest).Name;
            var isSensitive = request is ISensitiveRequest;

            if (isSensitive)
                _logger.LogInformation("Handling sensitive request {RequestName}", requestName);
            else
                _logger.LogInformation("Handling request {RequestName} {@Request}", requestName, request);

            try
            {
                var response = await next();
                _logger.LogInformation("Handled request {RequestName}", requestName);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling request {RequestName}", requestName);
                throw;
            }
        }
    }
}
