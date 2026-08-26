using Doctorly.Calendar.Domain.Events;
using Microsoft.EntityFrameworkCore;
namespace Doctorly.Calendar.Infrastructure.Persistence;
public sealed class CalendarDbContext(DbContextOptions<CalendarDbContext> options):DbContext(options)
{
 public DbSet<CalendarEvent> Events=>Set<CalendarEvent>(); public DbSet<OutboxMessage> OutboxMessages=>Set<OutboxMessage>();
    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        var calendarEvent =
            modelBuilder.Entity<CalendarEvent>();

        calendarEvent.ToTable("CalendarEvents");

        calendarEvent.HasKey(x => x.Id);

        calendarEvent
            .Property(x => x.Title)
            .HasMaxLength(200)
            .IsRequired();

        calendarEvent
            .Property(x => x.Description)
            .HasMaxLength(4000);

        calendarEvent
            .Property(x => x.StartTimeUtc)
            .HasConversion(
                value => value.UtcDateTime.Ticks,
                value => new DateTimeOffset(
                    new DateTime(
                        value,
                        DateTimeKind.Utc)));

        calendarEvent
            .Property(x => x.EndTimeUtc)
            .HasConversion(
                value => value.UtcDateTime.Ticks,
                value => new DateTimeOffset(
                    new DateTime(
                        value,
                        DateTimeKind.Utc)));

        calendarEvent
            .Property(x => x.Version)
            .IsConcurrencyToken();

        calendarEvent
            .HasMany(x => x.Attendees)
            .WithOne()
            .HasForeignKey(x => x.CalendarEventId)
            .OnDelete(DeleteBehavior.Cascade);

        calendarEvent
            .Navigation(x => x.Attendees)
            .UsePropertyAccessMode(
                PropertyAccessMode.Field);

        calendarEvent
            .HasIndex(x => x.StartTimeUtc);

        var attendee =
            modelBuilder.Entity<Attendee>();

        attendee.ToTable("Attendees");

        attendee.HasKey(x => x.Id);

        attendee
            .Property(x => x.Name)
            .HasMaxLength(150)
            .IsRequired();

        attendee
            .Property(x => x.Email)
            .HasMaxLength(254)
            .IsRequired();

        attendee
            .HasIndex(x => new
            {
                x.CalendarEventId,
                x.Email
            })
            .IsUnique();

        var outboxMessage =
            modelBuilder.Entity<OutboxMessage>();

        outboxMessage.ToTable("OutboxMessages");

        outboxMessage.HasKey(x => x.Id);

        outboxMessage
            .Property(x => x.Type)
            .HasMaxLength(120)
            .IsRequired();

        outboxMessage
            .Property(x => x.Payload)
            .IsRequired();

        outboxMessage
            .HasIndex(x => new
            {
                x.ProcessedAtUtc,
                x.OccurredAtUtc
            });
    }
}
