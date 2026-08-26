using System;
using System.Collections.Generic;
using System.Text;

namespace Doctorly.Calendar.Application.Common
{
    public record PagedResult<T>(IReadOnlyList<T> Items, int Page, int PageSize, int TotalCount);
}
