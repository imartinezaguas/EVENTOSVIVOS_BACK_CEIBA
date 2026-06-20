using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventosVivos.Domain.Entities;
using EventosVivos.Domain.Enums;
using EventosVivos.Domain.Interfaces;
using EventosVivos.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EventosVivos.Infrastructure.Repositories;

public class VenueRepository : IVenueRepository
{
    private readonly EventosVivosDbContext _context;

    public VenueRepository(EventosVivosDbContext context)
    {
        _context = context;
    }

    public async Task<Venue?> GetByIdAsync(int id)
    {
        return await _context.Venues.FindAsync(id);
    }

    public async Task<IEnumerable<Venue>> GetAllAsync()
    {
        return await _context.Venues.ToListAsync();
    }

    public async Task<(IEnumerable<Venue> Items, int TotalCount)> GetVenuesAsync(string? nameSearch, string? city, int pageNumber, int pageSize)
    {
        var query = _context.Venues.AsQueryable();

        if (!string.IsNullOrWhiteSpace(nameSearch))
            query = query.Where(v => v.Name.ToLower().Contains(nameSearch.ToLower()));

        if (!string.IsNullOrWhiteSpace(city))
            query = query.Where(v => v.City.ToLower().Contains(city.ToLower()));

        int totalCount = await query.CountAsync();

        var items = await query
            .OrderBy(v => v.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }
}
