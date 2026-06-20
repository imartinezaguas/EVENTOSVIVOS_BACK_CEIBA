using EventosVivos.Api.Extensions;
using EventosVivos.Api.Middlewares;
using EventosVivos.Application.Interfaces;
using EventosVivos.Application.Services;
using EventosVivos.Domain.Interfaces;
using EventosVivos.Infrastructure.Data;
using EventosVivos.Infrastructure.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure CORS
builder.Services.AddCustomCors();

// Configure Database (PostgreSQL)
builder.Services.AddDbContext<EventosVivosDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"),
        npgsqlOptions => npgsqlOptions.EnableRetryOnFailure(
            maxRetryCount: 3,
            maxRetryDelay: System.TimeSpan.FromSeconds(5),
            errorCodesToAdd: null))
           .UseLowerCaseNamingConvention());

// Dependency Injection - Repositories
builder.Services.AddScoped<IVenueRepository, VenueRepository>();
builder.Services.AddScoped<IEventRepository, EventRepository>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();

// Dependency Injection - Services
builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IVenueService, VenueService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

app.UseHttpsRedirection();
app.UseCors(CorsExtensions.AngularPolicy);
app.UseAuthorization();
app.MapControllers();

app.Run();
