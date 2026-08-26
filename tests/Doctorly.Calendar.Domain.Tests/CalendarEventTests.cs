using Doctorly.Calendar.Domain.Common;
using Doctorly.Calendar.Domain.Events;
using Xunit;
namespace Doctorly.Calendar.Domain.Tests;
public sealed class CalendarEventTests
{
 private static readonly DateTimeOffset Start=new(2026,9,1,8,0,0,TimeSpan.Zero);
 [Fact]public void Create_valid_event(){var e=Create();Assert.Equal(1,e.Version);Assert.Equal(AttendanceStatus.Pending,e.Attendees.Single().Status);}
 [Fact]public void Reject_duplicate_emails(){Assert.Throws<DomainException>(()=>CalendarEvent.Create("Visit","",Start,Start.AddMinutes(30),[("A","same@test.dev"),("B","SAME@test.dev")]));}
 [Fact]public void Reject_invalid_range(){Assert.Throws<DomainException>(()=>CalendarEvent.Create("Visit","",Start,Start,[("A","a@test.dev")]));}
 [Fact]public void Reject_stale_update(){var e=Create();Assert.Throws<ConcurrencyException>(()=>e.Update("New","",Start,Start.AddMinutes(45),[("A","a@test.dev")],99));}
 [Fact]public void Accept_and_increment_version(){var e=Create();e.Respond(e.Attendees.Single().Id,AttendanceStatus.Accepted,1);Assert.Equal(2,e.Version);Assert.Equal(AttendanceStatus.Accepted,e.Attendees.Single().Status);}
 [Fact]public void Cancel_prevents_update(){var e=Create();e.Cancel(1);Assert.Throws<DomainException>(()=>e.Update("New","",Start,Start.AddMinutes(45),[("A","a@test.dev")],2));}
 private static CalendarEvent Create()=>CalendarEvent.Create("Visit","Routine",Start,Start.AddMinutes(30),[("A","a@test.dev")]);
}
