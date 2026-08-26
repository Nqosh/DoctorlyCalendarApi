using Doctorly.Calendar.Domain.Common;

namespace Doctorly.Calendar.Domain.Events;

public sealed class CalendarEvent
{
    private readonly List<Attendee> _attendees = [];

    private CalendarEvent()
    {
        // Required by EF Core
    }

    public Guid Id { get; private set; }

    public string Title { get; private set; } = string.Empty;

    public string Description { get; private set; } = string.Empty;

    public DateTimeOffset StartTimeUtc { get; private set; }

    public DateTimeOffset EndTimeUtc { get; private set; }

    public CalendarEventStatus Status { get; private set; }

    public long Version { get; private set; }

    public DateTimeOffset CreatedAtUtc { get; private set; }

    public DateTimeOffset UpdatedAtUtc { get; private set; }

    public IReadOnlyCollection<Attendee> Attendees =>
        _attendees.AsReadOnly();

    public static CalendarEvent Create(
        string title,
        string description,
        DateTimeOffset start,
        DateTimeOffset end,
        IEnumerable<(string Name, string Email)> attendees)
    {
        var calendarEvent = new CalendarEvent
        {
            Id = Guid.NewGuid(),
            Status = CalendarEventStatus.Scheduled,
            Version = 1,
            CreatedAtUtc = DateTimeOffset.UtcNow,
            UpdatedAtUtc = DateTimeOffset.UtcNow
        };

        calendarEvent.Apply(
            title,
            description,
            start,
            end,
            attendees);

        return calendarEvent;
    }

    public void Update(
        string title,
        string description,
        DateTimeOffset start,
        DateTimeOffset end,
        IEnumerable<(string Name, string Email)> attendees,
        long expectedVersion)
    {
        EnsureMutable(expectedVersion);

        Apply(
            title,
            description,
            start,
            end,
            attendees);

        UpdateEventDetails();
    }

    public void Cancel(long expectedVersion)
    {
        EnsureMutable(expectedVersion);

        Status = CalendarEventStatus.Cancelled;

        UpdateEventDetails();
    }

    public void Respond(
        Guid attendeeId,
        AttendanceStatus response,
        long expectedVersion)
    {
        EnsureMutable(expectedVersion);

        var attendee =
            _attendees.SingleOrDefault(x => x.Id == attendeeId)
            ?? throw new DomainException(
                "Attendee not found.");

        attendee.Respond(response);

        UpdateEventDetails();
    }

    private void EnsureMutable(long expectedVersion)
    {
        if (Status == CalendarEventStatus.Cancelled)
        {
            throw new DomainException(
                "Cancelled events cannot be changed.");
        }

        if (Version != expectedVersion)
        {
            throw new ConcurrencyException(
                $"Expected version {expectedVersion}, current version is {Version}.");
        }
    }

    private void Apply(
        string title,
        string description,
        DateTimeOffset start,
        DateTimeOffset end,
        IEnumerable<(string Name, string Email)> attendees)
    {
        title = (title ?? string.Empty).Trim();
        description = (description ?? string.Empty).Trim();

        if (title.Length is < 1 or > 200)
        {
            throw new DomainException(
                "Title must be 1 to 200 characters.");
        }

        if (description.Length > 4000)
        {
            throw new DomainException(
                "Description cannot exceed 4000 characters.");
        }

        if (start.Offset != TimeSpan.Zero ||
            end.Offset != TimeSpan.Zero)
        {
            throw new DomainException(
                "Times must use UTC offset +00:00.");
        }

        if (end <= start)
        {
            throw new DomainException(
                "End time must be after start time.");
        }

        if (end - start > TimeSpan.FromDays(7))
        {
            throw new DomainException(
                "Duration cannot exceed seven days.");
        }

        var attendeeList =
            (attendees ?? [])
            .ToList();

        if (attendeeList.Count is < 1 or > 100)
        {
            throw new DomainException(
                "An event must have 1 to 100 attendees.");
        }

        if (attendeeList
                .Select(x => x.Email.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Count() != attendeeList.Count)
        {
            throw new DomainException(
                "Attendee emails must be unique.");
        }

        var existingAttendees =
            _attendees.ToDictionary(
                x => x.Email,
                StringComparer.OrdinalIgnoreCase);

        _attendees.Clear();

        foreach (var attendee in attendeeList)
        {
            if (existingAttendees.TryGetValue(
                    attendee.Email.Trim(),
                    out var existing))
            {
                existing.ChangeDetails(
                    attendee.Name,
                    attendee.Email);

                _attendees.Add(existing);
            }
            else
            {
                _attendees.Add(
                    new Attendee(
                        attendee.Name,
                        attendee.Email));
            }
        }

        Title = title;
        Description = description;
        StartTimeUtc = start;
        EndTimeUtc = end;
    }

    private void UpdateEventDetails()
    {
        Version++;
        UpdatedAtUtc = DateTimeOffset.UtcNow;
    }
}