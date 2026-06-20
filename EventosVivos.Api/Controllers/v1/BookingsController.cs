using EventosVivos.Application.DTOs;
using EventosVivos.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EventosVivos.Api.Controllers.v1;

[ApiController]
[Route("api/v1/[controller]")]
public class BookingsController : ControllerBase
{
    private readonly IBookingService _bookingService;

    public BookingsController(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateBooking([FromBody] CreateBookingDto dto)
    {
        try
        {
            var result = await _bookingService.CreateBookingAsync(dto);
            return CreatedAtAction(nameof(GetBookingById), new { id = result.Id }, result); 
        }
        catch (EventosVivos.Domain.Exceptions.DomainException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("{id}/confirm")]
    public async Task<IActionResult> ConfirmPayment(int id)
    {
        try
        {
            await _bookingService.ConfirmPaymentAsync(id);
            return Ok(new { message = "Pago confirmado y reserva completada." });
        }
        catch (EventosVivos.Domain.Exceptions.DomainException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("{id}/cancel")]
    public async Task<IActionResult> CancelBooking(int id)
    {
        try
        {
            await _bookingService.CancelBookingAsync(id);
            return Ok(new { message = "Reserva cancelada." });
        }
        catch (EventosVivos.Domain.Exceptions.DomainException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetBookings(
        [FromQuery] string? buyerNameSearch,
        [FromQuery] string? buyerEmailSearch,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = EventosVivos.Domain.Constants.GlobalConstants.Pagination.DefaultPageSize)
    {
        if (pageSize > EventosVivos.Domain.Constants.GlobalConstants.Pagination.MaxPageSize)
            pageSize = EventosVivos.Domain.Constants.GlobalConstants.Pagination.MaxPageSize;

        var bookings = await _bookingService.GetBookingsAsync(buyerNameSearch, buyerEmailSearch, pageNumber, pageSize);
        return Ok(bookings);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetBookingById(int id)
    {
        var booking = await _bookingService.GetBookingByIdAsync(id);
        if (booking == null)
            return NotFound(new { error = "La reserva no existe." });
            
        return Ok(booking);
    }
}
