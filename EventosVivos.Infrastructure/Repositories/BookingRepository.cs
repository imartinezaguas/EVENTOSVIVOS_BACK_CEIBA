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

public class BookingRepository : IBookingRepository
{
    private readonly EventosVivosDbContext _context;

    public BookingRepository(EventosVivosDbContext context)
    {
        _context = context;
    }

    public async Task<Booking?> GetByIdAsync(int id)
    {
        return await _context.Bookings.FindAsync(id);
    }

    public async Task<IEnumerable<Booking>> GetByEventIdAsync(int eventId)
    {
        return await _context.Bookings.Where(b => b.EventId == eventId).ToListAsync();
    }

    public async Task AddAsync(Booking booking)
    {
        await _context.Bookings.AddAsync(booking);
    }

    public Task UpdateAsync(Booking booking)
    {
        _context.Bookings.Update(booking);
        return Task.CompletedTask;
    }

    public async Task<int> GetConfirmedAndPenaltyTicketsCountForEventAsync(int eventId)
    {
        var bookings = await _context.Bookings
            .Include(b => b.Event)
            .Where(b => b.EventId == eventId)
            .ToListAsync();
            
        return bookings
            .Where(b => b.Status == BookingStatus.Confirmada || b.IsPenaltyCancellation(b.Event))
            .Sum(b => b.Quantity);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<(IEnumerable<Booking> Items, int TotalCount)> GetBookingsAsync(string? buyerNameSearch, string? buyerEmailSearch, int pageNumber, int pageSize)
    {
        var query = _context.Bookings.AsQueryable();

        if (!string.IsNullOrWhiteSpace(buyerNameSearch))
            query = query.Where(b => b.BuyerName.ToLower().Contains(buyerNameSearch.ToLower()));

        if (!string.IsNullOrWhiteSpace(buyerEmailSearch))
            query = query.Where(b => b.BuyerEmail.ToLower().Contains(buyerEmailSearch.ToLower()));

        int totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(b => b.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }
}
