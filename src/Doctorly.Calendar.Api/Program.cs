using System.Text.Json.Serialization;
using Doctorly.Calendar.Api;
using Doctorly.Calendar.Application;
using Doctorly.Calendar.Infrastructure;
using Doctorly.Calendar.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
var builder=WebApplication.CreateBuilder(args);
builder.Services.AddApplication().AddInfrastructure(builder.Configuration);
builder.Services.AddControllers().AddJsonOptions(o=>o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddProblemDetails();builder.Services.AddExceptionHandler<ApiExceptionHandler>();builder.Services.AddEndpointsApiExplorer();builder.Services.AddSwaggerGen(o=>o.SwaggerDoc("v1",new(){Title="Doctorly Calendar API",Version="v1",Description="Calendar and appointment management API."}));builder.Services.AddHealthChecks();
var app=builder.Build();app.UseExceptionHandler();
if(app.Environment.IsDevelopment()){await using var scope=app.Services.CreateAsyncScope();await scope.ServiceProvider.GetRequiredService<CalendarDbContext>().Database.EnsureCreatedAsync();app.UseSwagger();app.UseSwaggerUI();}
app.UseHttpsRedirection();app.MapControllers();app.MapHealthChecks("/health");app.Run();
public partial class Program;
