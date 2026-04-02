using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace AiReviewHub.Application.Common.Behaviors
{
    public class PerformanceBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
    {
        private const int DefaultThresholdMs = 500;
        private readonly ILogger<PerformanceBehavior<TRequest, TResponse>> _logger;
        private readonly int _thresholdMs;

        public PerformanceBehavior(ILogger<PerformanceBehavior<TRequest, TResponse>> logger, IConfiguration configuration)
        {
            _logger = logger;
            _thresholdMs = configuration.GetValue<int?>("Performance:SlowRequestThresholdMs") ?? DefaultThresholdMs;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            var stopwatch = Stopwatch.StartNew();
            var response = await next();
            stopwatch.Stop();

            if (stopwatch.ElapsedMilliseconds > _thresholdMs)
            {
                _logger.LogWarning(
                    "Long running request: {RequestName} ({ElapsedMilliseconds} ms) — threshold: {ThresholdMs} ms",
                    typeof(TRequest).Name,
                    stopwatch.ElapsedMilliseconds,
                    _thresholdMs);
            }

            return response;
        }
    }
}
