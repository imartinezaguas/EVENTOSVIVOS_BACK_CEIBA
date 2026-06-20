using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventosVivos.Domain.Entities;
using EventosVivos.Domain.Enums;
using EventosVivos.Domain.Interfaces;
using EventosVivos.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EventosVivos.Infrastructure.Repositories;

public class EventRepository : IEventRepository
{
    private readonly EventosVivosDbContext _context;

    public EventRepository(EventosVivosDbContext context)
    {
        _context = context;
    }

    public async Task<Event?> GetByIdAsync(int id)
    {
        return await _context.Events.Include(e => e.Venue).FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<(IEnumerable<Event> Items, int TotalCount)> GetEventsAsync(EventType? type, DateTime? startDate, DateTime? endDate, int? venueId, EventStatus? status, string? titleSearch, int pageNumber, int pageSize)
    {
        var query = _context.Events.Include(e => e.Venue).AsQueryable();

        if (type.HasValue)
            query = query.Where(e => e.Type == type.Value);

        if (startDate.HasValue)
        {
            var utcStartDate = startDate.Value.Kind == DateTimeKind.Unspecified 
                ? DateTime.SpecifyKind(startDate.Value, DateTimeKind.Utc) 
                : startDate.Value.ToUniversalTime();
            query = query.Where(e => e.StartDate >= utcStartDate);
        }

        if (endDate.HasValue)
        {
            var utcEndDate = endDate.Value.Kind == DateTimeKind.Unspecified 
                ? DateTime.SpecifyKind(endDate.Value, DateTimeKind.Utc) 
                : endDate.Value.ToUniversalTime();
            // We want events that start on or before the end date of the filter
            query = query.Where(e => e.StartDate <= utcEndDate);
        }

        if (venueId.HasValue)
            query = query.Where(e => e.VenueId == venueId.Value);

        if (status.HasValue)
            query = query.Where(e => e.Status == status.Value);

        if (!string.IsNullOrWhiteSpace(titleSearch))
            query = query.Where(e => e.Title.ToLower().Contains(titleSearch.ToLower()));

        int totalCount = await query.CountAsync();

        var items = await query
            .OrderBy(e => e.StartDate)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<bool> HasOverlappingEventsAsync(int venueId, DateTime startDate, DateTime endDate)
    {
        return await _context.Events
            .AnyAsync(e => e.VenueId == venueId && e.Status == EventStatus.Activo && 
                           e.StartDate < endDate && e.EndDate > startDate);
    }

    public async Task AddAsync(Event @event)
    {
        await _context.Events.AddAsync(@event);
    }

    public Task UpdateAsync(Event @event)
    {
        _context.Events.Update(@event);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
