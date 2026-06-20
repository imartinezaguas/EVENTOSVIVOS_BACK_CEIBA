using System.Collections.Generic;
using System.Threading.Tasks;
using EventosVivos.Domain.Entities;

namespace EventosVivos.Domain.Interfaces;

public interface IBookingRepository
{
    Task<Booking?> GetByIdAsync(int id);
    Task<IEnumerable<Booking>> GetByEventIdAsync(int eventId);
    Task AddAsync(Booking booking);
    Task UpdateAsync(Booking booking);
    Task<int> GetConfirmedAndPenaltyTicketsCountForEventAsync(int eventId);
    Task<(IEnumerable<Booking> Items, int TotalCount)> GetBookingsAsync(string? buyerNameSearch, string? buyerEmailSearch, int pageNumber, int pageSize);
    Task SaveChangesAsync();
}
