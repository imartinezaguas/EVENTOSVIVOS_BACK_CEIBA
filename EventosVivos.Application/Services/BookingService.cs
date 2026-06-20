using System.Threading.Tasks;
using EventosVivos.Application.DTOs;
using EventosVivos.Application.Interfaces;
using EventosVivos.Domain.Constants;
using EventosVivos.Domain.Entities;
using EventosVivos.Domain.Exceptions;
using EventosVivos.Domain.Interfaces;

namespace EventosVivos.Application.Services;

public class BookingService : IBookingService
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IEventRepository _eventRepository;

    public BookingService(IBookingRepository bookingRepository, IEventRepository eventRepository)
    {
        _bookingRepository = bookingRepository;
        _eventRepository = eventRepository;
    }

    public async Task<BookingDto> CreateBookingAsync(CreateBookingDto dto)
    {
        var @event = await _eventRepository.GetByIdAsync(dto.EventId) 
            ?? throw new DomainException(GlobalConstants.ErrorMessages.EventNotFound);

        // Verificar disponibilidad
        int ticketsTaken = await _bookingRepository.GetConfirmedAndPenaltyTicketsCountForEventAsync(dto.EventId);
        if (ticketsTaken + dto.NumberOfTickets > @event.MaxCapacity)
        {
            throw new DomainException(GlobalConstants.ErrorMessages.NotEnoughTickets);
        }

        var booking = new Booking(dto.EventId, dto.NumberOfTickets, dto.BuyerName, dto.BuyerEmail, @event);

        await _bookingRepository.AddAsync(booking);
        await _bookingRepository.SaveChangesAsync();

        return MapToDto(booking);
    }

    public async Task ConfirmPaymentAsync(int bookingId)
    {
        var booking = await _bookingRepository.GetByIdAsync(bookingId) 
            ?? throw new DomainException("La reserva no existe.");

        booking.ConfirmPayment();

        await _bookingRepository.UpdateAsync(booking);
        await _bookingRepository.SaveChangesAsync();
    }

    public async Task CancelBookingAsync(int bookingId)
    {
        var booking = await _bookingRepository.GetByIdAsync(bookingId) 
            ?? throw new DomainException("La reserva no existe.");

        var @event = await _eventRepository.GetByIdAsync(booking.EventId)
            ?? throw new DomainException(GlobalConstants.ErrorMessages.EventNotFound);

        booking.Cancel(@event);

        await _bookingRepository.UpdateAsync(booking);
        await _bookingRepository.SaveChangesAsync();
    }

    public async Task<PagedResultDto<BookingDto>> GetBookingsAsync(string? buyerNameSearch, string? buyerEmailSearch, int pageNumber, int pageSize)
    {
        var (bookings, totalCount) = await _bookingRepository.GetBookingsAsync(buyerNameSearch, buyerEmailSearch, pageNumber, pageSize);

        return new PagedResultDto<BookingDto>
        {
            Items = System.Linq.Enumerable.Select(bookings, MapToDto),
            TotalRecords = totalCount,
            Page = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<BookingDto?> GetBookingByIdAsync(int id)
    {
        var booking = await _bookingRepository.GetByIdAsync(id);
        if (booking == null) return null;
        return MapToDto(booking);
    }

    private static BookingDto MapToDto(Booking booking)
    {
        return new BookingDto
        {
            Id = booking.Id,
            EventId = booking.EventId,
            NumberOfTickets = booking.Quantity,
            BuyerName = booking.BuyerName,
            BuyerEmail = booking.BuyerEmail,
            Status = booking.Status.ToString(),
            ReservationCode = booking.ReservationCode,
            CreatedAt = booking.CreatedAt
        };
    }
}
