using AiReviewHub.Application.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace AiReviewHub.Tests.Helpers
{
    public class FakeCurrentUserService : ICurrentUserService
    {
        public Guid UserId { get; set; } = Guid.NewGuid();
        public bool IsAuthenticated { get; set; } = true;
    }
}
