using System;
using EventosVivos.Domain.Enums;

namespace EventosVivos.Application.DTOs;

public class CreateEventDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int VenueId { get; set; }
    public int MaxCapacity { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal Price { get; set; }
    public EventType Type { get; set; }
}

public class EventVenueDto
{
    public string Name { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
}

public class EventDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int VenueId { get; set; }
    public EventVenueDto? Venue { get; set; }
    public int MaxCapacity { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal Price { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}

public class OccupancyReportDto
{
    public int EventId { get; set; }
    public int TotalConfirmedTickets { get; set; }
    public int RemainingTickets { get; set; }
    public double OccupancyPercentage { get; set; }
    public decimal TotalRevenue { get; set; }
    public string EventStatus { get; set; } = string.Empty;
}
