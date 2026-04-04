using System;
using System.Collections.Generic;
using System.Text;

namespace AiReviewHub.Application.Common.Models
{
    public record PagedResult<T>(
        IReadOnlyList<T> Data,
        PaginationMeta Meta
    );

    public record PaginationMeta(int Total, int Page, int PageSize, int TotalPages)
    {
        public static PaginationMeta Create(int total, int page, int pageSize) =>
            new(
                Total: total,
                Page: page,
                PageSize: pageSize,
                TotalPages: (int)Math.Ceiling((double)total / pageSize)
            );
    }
}
