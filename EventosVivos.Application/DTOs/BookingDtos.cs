using System;

namespace EventosVivos.Application.DTOs;

public class CreateBookingDto
{
    public int EventId { get; set; }
    public int NumberOfTickets { get; set; }
    public string BuyerName { get; set; } = string.Empty;
    public string BuyerEmail { get; set; } = string.Empty;
}

public class BookingDto
{
    public int Id { get; set; }
    public int EventId { get; set; }
    public int NumberOfTickets { get; set; }
    public string BuyerName { get; set; } = string.Empty;
    public string BuyerEmail { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? ReservationCode { get; set; }
    public DateTime CreatedAt { get; set; }
}
