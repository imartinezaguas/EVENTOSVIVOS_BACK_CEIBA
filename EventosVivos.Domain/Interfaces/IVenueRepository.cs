using System.Collections.Generic;
using System.Threading.Tasks;
using EventosVivos.Domain.Entities;

namespace EventosVivos.Domain.Interfaces;

public interface IVenueRepository
{
    Task<Venue?> GetByIdAsync(int id);
    Task<IEnumerable<Venue>> GetAllAsync();
    Task<(IEnumerable<Venue> Items, int TotalCount)> GetVenuesAsync(string? nameSearch, string? city, int pageNumber, int pageSize);
}
