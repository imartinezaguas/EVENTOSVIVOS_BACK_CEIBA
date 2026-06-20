using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventosVivos.Application.DTOs;
using EventosVivos.Application.Interfaces;
using EventosVivos.Domain.Constants;
using EventosVivos.Domain.Entities;
using EventosVivos.Domain.Enums;
using EventosVivos.Domain.Exceptions;
using EventosVivos.Domain.Interfaces;

namespace EventosVivos.Application.Services;

public class EventService : IEventService
{
    private readonly IEventRepository _eventRepository;
    private readonly IVenueRepository _venueRepository;
    private readonly IBookingRepository _bookingRepository;

    public EventService(
        IEventRepository eventRepository,
        IVenueRepository venueRepository,
        IBookingRepository bookingRepository)
    {
        _eventRepository = eventRepository;
        _venueRepository = venueRepository;
        _bookingRepository = bookingRepository;
    }

    public async Task<EventDto> CreateEventAsync(CreateEventDto dto)
    {
        var venue = await _venueRepository.GetByIdAsync(dto.VenueId) 
            ?? throw new DomainException(GlobalConstants.ErrorMessages.VenueNotFound);

        // RN-01: Capacidad del venue
        if (dto.MaxCapacity > venue.Capacity)
        {
            throw new DomainException(GlobalConstants.ErrorMessages.CapacityExceeded);
        }

        // RN-02: Superposición de venues
        bool isOverlap = await _eventRepository.HasOverlappingEventsAsync(dto.VenueId, dto.StartDate, dto.EndDate);
        if (isOverlap)
        {
            throw new DomainException(GlobalConstants.ErrorMessages.VenueOverlap);
        }

        var newEvent = new Event(
            dto.Title,
            dto.Description,
            dto.VenueId,
            dto.MaxCapacity,
            dto.StartDate,
            dto.EndDate,
            dto.Price,
            dto.Type
        );

        await _eventRepository.AddAsync(newEvent);
        await _eventRepository.SaveChangesAsync();

        return MapToDto(newEvent);
    }

    public async Task<PagedResultDto<EventDto>> GetEventsAsync(EventType? type, DateTime? startDate, DateTime? endDate, int? venueId, EventStatus? status, string? titleSearch, int pageNumber, int pageSize)
    {
        var (events, totalCount) = await _eventRepository.GetEventsAsync(type, startDate, endDate, venueId, status, titleSearch, pageNumber, pageSize);
        
        // RN-06: Marcar como completado si la fecha actual supera hora de fin (actualización pasiva)
        bool hasChanges = false;
        foreach(var ev in events)
        {
            if (ev.Status == EventStatus.Activo && DateTime.UtcNow > ev.EndDate)
            {
                ev.MarkAsCompleted();
                await _eventRepository.UpdateAsync(ev);
                hasChanges = true;
            }
        }
        
        if (hasChanges)
        {
            await _eventRepository.SaveChangesAsync();
        }

        return new PagedResultDto<EventDto>
        {
            Items = events.Select(MapToDto),
            TotalRecords = totalCount,
            Page = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<EventDto?> GetEventByIdAsync(int id)
    {
        var @event = await _eventRepository.GetByIdAsync(id);
        if (@event == null) return null;
        return MapToDto(@event);
    }

    public async Task<OccupancyReportDto?> GetOccupancyReportAsync(int eventId)
    {
        var @event = await _eventRepository.GetByIdAsync(eventId);
        if (@event == null) return null;

        // Contar reservas confirmadas y canceladas con penalidad
        int ticketsTaken = await _bookingRepository.GetConfirmedAndPenaltyTicketsCountForEventAsync(eventId);

        int remaining = @event.MaxCapacity - ticketsTaken;
        double occupancyPct = @event.MaxCapacity > 0 ? (ticketsTaken * 100.0) / @event.MaxCapacity : 0;
        decimal totalRevenue = ticketsTaken * @event.Price;

        return new OccupancyReportDto
        {
            EventId = @event.Id,
            TotalConfirmedTickets = ticketsTaken,
            RemainingTickets = remaining,
            OccupancyPercentage = occupancyPct,
            TotalRevenue = totalRevenue,
            EventStatus = @event.Status.ToString()
        };
    }

    private static EventDto MapToDto(Event @event)
    {
        return new EventDto
        {
            Id = @event.Id,
            Title = @event.Title,
            Description = @event.Description,
            VenueId = @event.VenueId,
            Venue = @event.Venue != null ? new EventVenueDto
            {
                Name = @event.Venue.Name,
                City = @event.Venue.City
            } : null,
            MaxCapacity = @event.MaxCapacity,
            StartDate = @event.StartDate,
            EndDate = @event.EndDate,
            Price = @event.Price,
            Type = @event.Type.ToString(),
            Status = @event.Status.ToString()
        };
    }
}
