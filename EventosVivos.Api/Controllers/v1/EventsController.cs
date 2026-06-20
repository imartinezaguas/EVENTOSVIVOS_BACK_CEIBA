using EventosVivos.Application.DTOs;
using EventosVivos.Application.Interfaces;
using EventosVivos.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace EventosVivos.Api.Controllers.v1;

[ApiController]
[Route("api/v1/[controller]")]
public class EventsController : ControllerBase
{
    private readonly IEventService _eventService;

    public EventsController(IEventService eventService)
    {
        _eventService = eventService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateEvent([FromBody] CreateEventDto dto)
    {
        try
        {
            var result = await _eventService.CreateEventAsync(dto);
            return CreatedAtAction(nameof(GetEventById), new { id = result.Id }, result);
        }
        catch (EventosVivos.Domain.Exceptions.DomainException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetEvents(
        [FromQuery] EventType? type,
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate,
        [FromQuery] int? venueId,
        [FromQuery] EventStatus? status,
        [FromQuery] string? titleSearch,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = EventosVivos.Domain.Constants.GlobalConstants.Pagination.DefaultPageSize)
    {
        if (pageSize > EventosVivos.Domain.Constants.GlobalConstants.Pagination.MaxPageSize)
            pageSize = EventosVivos.Domain.Constants.GlobalConstants.Pagination.MaxPageSize;
            
        var events = await _eventService.GetEventsAsync(type, startDate, endDate, venueId, status, titleSearch, pageNumber, pageSize);
        return Ok(events);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetEventById(int id)
    {
        var @event = await _eventService.GetEventByIdAsync(id);
        if (@event == null)
            return NotFound(new { error = "El evento no existe." });
            
        return Ok(@event);
    }

    [HttpGet("{id}/occupancy-report")]
    public async Task<IActionResult> GetOccupancyReport(int id)
    {
        var report = await _eventService.GetOccupancyReportAsync(id);
        if (report == null)
        {
            return NotFound(new { error = "El evento no existe." });
        }
        return Ok(report);
    }
}
