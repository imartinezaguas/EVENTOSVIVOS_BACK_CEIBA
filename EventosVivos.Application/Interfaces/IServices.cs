using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventosVivos.Application.DTOs;
using EventosVivos.Domain.Enums;

namespace EventosVivos.Application.Interfaces;

public interface IEventService
{
    Task<EventDto> CreateEventAsync(CreateEventDto dto);
    Task<PagedResultDto<EventDto>> GetEventsAsync(EventType? type, DateTime? startDate, DateTime? endDate, int? venueId, EventStatus? status, string? titleSearch, int pageNumber, int pageSize);
    Task<EventDto?> GetEventByIdAsync(int id);
    Task<OccupancyReportDto?> GetOccupancyReportAsync(int eventId);
}

public interface IBookingService
{
    Task<BookingDto> CreateBookingAsync(CreateBookingDto dto);
    Task ConfirmPaymentAsync(int bookingId);
    Task CancelBookingAsync(int bookingId);
    Task<BookingDto?> GetBookingByIdAsync(int id);
    Task<PagedResultDto<BookingDto>> GetBookingsAsync(string? buyerNameSearch, string? buyerEmailSearch, int pageNumber, int pageSize);
}

public interface IVenueService
{
    Task<PagedResultDto<VenueDto>> GetVenuesAsync(string? nameSearch, string? city, int pageNumber, int pageSize);
}
