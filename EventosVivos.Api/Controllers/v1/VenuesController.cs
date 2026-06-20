using EventosVivos.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EventosVivos.Api.Controllers.v1;

[ApiController]
[Route("api/v1/[controller]")]
public class VenuesController : ControllerBase
{
    private readonly IVenueService _venueService;

    public VenuesController(IVenueService venueService)
    {
        _venueService = venueService;
    }

    [HttpGet]
    public async Task<IActionResult> GetVenues(
        [FromQuery] string? nameSearch,
        [FromQuery] string? city,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = EventosVivos.Domain.Constants.GlobalConstants.Pagination.DefaultPageSize)
    {
        if (pageSize > EventosVivos.Domain.Constants.GlobalConstants.Pagination.MaxPageSize)
            pageSize = EventosVivos.Domain.Constants.GlobalConstants.Pagination.MaxPageSize;

        var venues = await _venueService.GetVenuesAsync(nameSearch, city, pageNumber, pageSize);
        return Ok(venues);
    }
}
