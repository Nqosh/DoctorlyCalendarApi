using Doctorly.Calendar.Application.Common;
using Doctorly.Calendar.Domain.Events;
namespace Doctorly.Calendar.Application.Abstractions;
public interface IEventRepository
{
 Task<CalendarEvent?> GetAsync(Guid id,bool tracking,CancellationToken ct);
 Task AddAsync(CalendarEvent item,CancellationToken ct);
 Task<PagedResult<CalendarEvent>> SearchAsync(EventSearch search,CancellationToken ct);
 Task<bool> HasConflictAsync(IEnumerable<string> emails,DateTimeOffset start,DateTimeOffset end,Guid? excludingId,CancellationToken ct);
}



