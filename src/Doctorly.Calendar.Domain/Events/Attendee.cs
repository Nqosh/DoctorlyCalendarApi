using System.Net.Mail;
using Doctorly.Calendar.Domain.Common;

namespace Doctorly.Calendar.Domain.Events;

public class Attendee
{
    private Attendee()
    {
        // Required by EF Core
    }

    public Attendee(
        string name,
        string email)
    {
        Id = Guid.NewGuid();

        ChangeDetails(name, email);

        Status = AttendanceStatus.Pending;
    }

    public Guid Id { get; private set; }

    public Guid CalendarEventId { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string Email { get; private set; } = string.Empty;

    public AttendanceStatus Status { get; private set; }

    internal void ChangeDetails(string name, string email)
    {
        name = (name ?? string.Empty).Trim();

        email = (email ?? string.Empty)
            .Trim()
            .ToLowerInvariant();

        if (name.Length is < 1 or > 150)
        {
            throw new DomainException(
                "Attendee name must be 1 to 150 characters.");
        }

        if (email.Length is < 3 or > 254 ||
            !MailAddress.TryCreate(email, out _))
        {
            throw new DomainException(
                "A valid attendee email is required.");
        }

        Name = name;
        Email = email;
    }

    internal void Respond(AttendanceStatus response)
    {
        if (response == AttendanceStatus.Pending)
        {
            throw new DomainException(
                "Response must be Accepted or Rejected.");
        }

        Status = response;
    }
}