using System.Linq;
using System.Threading.Tasks;
using EventosVivos.Application.DTOs;
using EventosVivos.Application.Interfaces;
using EventosVivos.Domain.Interfaces;

namespace EventosVivos.Application.Services;

public class VenueService : IVenueService
{
    private readonly IVenueRepository _venueRepository;

    public VenueService(IVenueRepository venueRepository)
    {
        _venueRepository = venueRepository;
    }

    public async Task<PagedResultDto<VenueDto>> GetVenuesAsync(string? nameSearch, string? city, int pageNumber, int pageSize)
    {
        var (venues, totalCount) = await _venueRepository.GetVenuesAsync(nameSearch, city, pageNumber, pageSize);

        return new PagedResultDto<VenueDto>
        {
            Items = venues.Select(v => new VenueDto
            {
                Id = v.Id,
                Name = v.Name,
                Capacity = v.Capacity,
                City = v.City
            }),
            TotalRecords = totalCount,
            Page = pageNumber,
            PageSize = pageSize
        };
    }
}
