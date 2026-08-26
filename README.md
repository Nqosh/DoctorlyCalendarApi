🩺 Doctorly Calendar & Appointment Management API

A modern Calendar and Appointment Management API built using .NET 10, ASP.NET Core, Entity Framework Core, and Clean Architecture.

The solution provides event scheduling, attendee management, availability checking, optimistic concurrency control, notification processing, iCalendar generation, OpenAPI documentation, 
and comprehensive automated plus manual testing.

---

## 🚀 Features

- Calendar Management
- Create Calendar Events
- Update Calendar Events
- Cancel Calendar Events
- Retrieve Event Details
- Search Events
- Event Pagination
- Event Filtering
- Attendee Management
- Add Attendees
- Accept Invitations
- Reject Invitations
- Track Attendee Response Status
- Availability Checking
- Detect Scheduling Conflicts
- Attendee Availability Validation
- Overlapping Appointment Detection
- Reliability & Notifications
- Transactional Outbox Pattern
- Background Notification Processing
- Notification Provider Abstraction
- iCalendar (.ics) Generation
- Extensible Notification Architecture (Email, MQ, Service Bus, etc.)

### API Features
- RESTful API Design
- OpenAPI / Swagger Documentation
- Dependency Injection
- Global Exception Handling
- Optimistic Concurrency
- Validation Rules
- Pagination Support
- Testing
- Domain Unit Tests
- API Integration Tests
- Manual Test Execution Guide
- Swagger-Based Verification

🏗️ Architecture

The solution follows Clean Architecture principles and separates responsibilities into distinct layers.
```
Doctorly.Calendar.sln
│
├── Doctorly.Calendar.Api
│   ├── Controllers
│   ├── ApiExceptionHandler
│   ├── Program.cs
│   └── appsettings.json
│
├── Doctorly.Calendar.Application
│   │
│   ├── Abstractions
│   │   ├── IEventRepository
│   │   ├── IUnitOfWork
│   │   ├── IOutbox
│   │   └── INotificationSender
│   │
│   ├── Common
│   │   ├── EventSearch
│   │   └── PagedResult
│   │
│   └── Events
│       ├── Requests
│       ├── Responses
│       └── EventService
│
├── Doctorly.Calendar.Domain
│   │
│   ├── Common
│   │   ├── DomainException
│   │   └── ConcurrencyException
│   │
│   └── Events
│       ├── CalendarEvent
│       ├── Attendee
│       ├── CalendarEventStatus
│       └── AttendanceStatus
│
├── Doctorly.Calendar.Infrastructure
│   │
│   ├── Persistence
│   │   ├── CalendarDbContext
│   │   ├── EventRepository
│   │   ├── UnitOfWork
│   │   ├── Outbox
│   │   └── OutboxMessage
│   │
│   ├── Notifications
│   │   ├── LoggingNotificationSender
│   │   ├── OutboxWorker
│   │   └── ICalendarDocument
│   │
│   └── DependencyInjection
│
├── Doctorly.Calendar.Domain.Tests
│
└── Doctorly.Calendar.Api.Tests
```

🎯 Design Principles

The solution is based on:

- Clean Architecture
- SOLID Principles
- Separation of Concerns
- Dependency Inversion
- Repository Pattern
- Unit of Work Pattern
- Transactional Outbox Pattern
- Optimistic Concurrency Control
- Domain-Driven Design Concepts
- Testability
- Maintainability
- Scalability

🛠 Technology Stack
- Backend
- .NET 10
- ASP.NET Core Web API
- C#
- Entity Framework Core
- SQLite
- Swagger / OpenAPI
- Testing
- xUnit
- ASP.NET Core WebApplicationFactory
- EF Core InMemory Database

### Developemnt Tools
- Visual Studio 2022
- Git
- GitHub

📋 Functional Requirements Implemented

- Requirement	StatusCreate Event	✅
- Update Event	✅
- Cancel Event	✅
- Retrieve Event	✅
- Search Events	✅
- Attendee Responses	✅
- Availability Checking	✅
- Optimistic Concurrency	✅
- Pagination	✅
- Validation Rules	✅
- Field Size Limits	✅
- Notification Capability	✅
- iCalendar Support	✅
- Automated Testing	✅
- Swagger Documentation	✅


🔔 Notification Architecture

- The solution implements the Transactional Outbox Pattern.

## Workflow
```
Create / Update / Cancel Event
            │
            ▼
       Outbox Record
            │
            ▼
      Outbox Worker
            │
            ▼
  Notification Sender
            │
            ▼
   Email / Log

```
 
🧪 Testing Strategy
Unit Tests

Domain behavior is tested for:

- Event Creation
- Invalid Date Ranges
- Duplicate Attendees
- Concurrency Validation
- Accept Response
- Reject Response
- Event Cancellation
- iCalendar Generation
- Integration Tests

API coverage includes:

- Create Event
- Get Event
- Update Event
- Cancel Event
- Search Events
- Availability Checking
- Validation Errors
- Concurrency Errors

⚙️ Getting Started
Prerequisites

Install:
- .NET 8 SDK
- SQL Server
- Visual Studio 2022 - 2025
### Clone Repository

```bash
git clone https://github.com/Nqosh/DoctorlyCalendarApi.git
```

### Restore Packages

```bash
dotnet restore
```

### Build Solution

```bash
dotnet build
```

### Run API

```bash
dotnet run --project Doctorly.Calendar.Api
