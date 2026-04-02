using System;
using System.Collections.Generic;
using System.Text;

namespace AiReviewHub.Application.Common.Interfaces
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(Guid userId, string email);
    }
}
