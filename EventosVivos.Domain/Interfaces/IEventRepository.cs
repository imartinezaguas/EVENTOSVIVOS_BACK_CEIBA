using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventosVivos.Domain.Entities;
using EventosVivos.Domain.Enums;

namespace EventosVivos.Domain.Interfaces;

public interface IEventRepository
{
    Task<Event?> GetByIdAsync(int id);
    Task<(IEnumerable<Event> Items, int TotalCount)> GetEventsAsync(EventType? type, DateTime? startDate, DateTime? endDate, int? venueId, EventStatus? status, string? titleSearch, int pageNumber, int pageSize);
    Task<bool> HasOverlappingEventsAsync(int venueId, DateTime startDate, DateTime endDate);
    Task AddAsync(Event @event);
    Task UpdateAsync(Event @event);
    Task SaveChangesAsync();
}
