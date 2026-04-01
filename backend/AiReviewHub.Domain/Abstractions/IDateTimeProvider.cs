using System;
using System.Collections.Generic;
using System.Text;

namespace AiReviewHub.Domain.Abstractions
{
    public interface IDateTimeProvider
    {
        DateTime UtcNow { get; }
    }
}
