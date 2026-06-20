using System;
using System.Threading.Tasks;
using EventosVivos.Application.DTOs;
using EventosVivos.Application.Services;
using EventosVivos.Domain.Constants;
using EventosVivos.Domain.Entities;
using EventosVivos.Domain.Enums;
using EventosVivos.Domain.Exceptions;
using EventosVivos.Domain.Interfaces;
using Moq;
using Xunit;

namespace EventosVivos.Tests.Application;

public class BookingServiceTests
{
    private readonly Mock<IBookingRepository> _bookingRepoMock;
    private readonly Mock<IEventRepository> _eventRepoMock;
    private readonly BookingService _sut;

    public BookingServiceTests()
    {
        _bookingRepoMock = new Mock<IBookingRepository>();
        _eventRepoMock = new Mock<IEventRepository>();

        _sut = new BookingService(
            _bookingRepoMock.Object,
            _eventRepoMock.Object);
    }

    private Event CreateValidEvent(int maxCapacity = 100)
    {
        var startDate = DateTime.UtcNow.AddDays(5).Date.AddHours(14);
        if (startDate.DayOfWeek == DayOfWeek.Saturday || startDate.DayOfWeek == DayOfWeek.Sunday)
            startDate = startDate.AddDays(2);
            
        return new Event("Test Event", "Description Test", 1, maxCapacity, startDate, startDate.AddHours(2), 50, EventType.Conferencia);
    }

    [Fact]
    public async Task CreateBookingAsync_ValidBooking_ReturnsDto()
    {
        // Arrange
        var dto = new CreateBookingDto { EventId = 1, NumberOfTickets = 2, BuyerName = "Juan", BuyerEmail = "juan@test.com" };
        var @event = CreateValidEvent();
        
        _eventRepoMock.Setup(x => x.GetByIdAsync(dto.EventId)).ReturnsAsync(@event);
        _bookingRepoMock.Setup(x => x.GetConfirmedAndPenaltyTicketsCountForEventAsync(dto.EventId)).ReturnsAsync(0);

        // Act
        var result = await _sut.CreateBookingAsync(dto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(dto.NumberOfTickets, result.NumberOfTickets);
        _bookingRepoMock.Verify(x => x.AddAsync(It.IsAny<Booking>()), Times.Once);
        _bookingRepoMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateBookingAsync_NotEnoughTickets_ThrowsDomainException()
    {
        // Arrange
        var dto = new CreateBookingDto { EventId = 1, NumberOfTickets = 10, BuyerName = "Juan", BuyerEmail = "juan@test.com" };
        var @event = CreateValidEvent(maxCapacity: 100);
        
        _eventRepoMock.Setup(x => x.GetByIdAsync(dto.EventId)).ReturnsAsync(@event);
        _bookingRepoMock.Setup(x => x.GetConfirmedAndPenaltyTicketsCountForEventAsync(dto.EventId)).ReturnsAsync(95); // 95 + 10 = 105 > 100

        // Act & Assert
        var ex = await Assert.ThrowsAsync<DomainException>(() => _sut.CreateBookingAsync(dto));
        Assert.Equal(GlobalConstants.ErrorMessages.NotEnoughTickets, ex.Message);
    }

    [Fact]
    public async Task ConfirmPaymentAsync_ValidBooking_ConfirmsAndUpdates()
    {
        // Arrange
        var @event = CreateValidEvent();
        var booking = new Booking(1, 2, "Juan", "juan@test.com", @event);
        
        _bookingRepoMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(booking);

        // Act
        await _sut.ConfirmPaymentAsync(1);

        // Assert
        Assert.Equal(BookingStatus.Confirmada, booking.Status);
        Assert.NotNull(booking.ReservationCode);
        _bookingRepoMock.Verify(x => x.UpdateAsync(booking), Times.Once);
        _bookingRepoMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CancelBookingAsync_ValidBooking_CancelsAndUpdates()
    {
        // Arrange
        var @event = CreateValidEvent();
        var booking = new Booking(1, 2, "Juan", "juan@test.com", @event);
        booking.ConfirmPayment(); // Confirm first
        
        _bookingRepoMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(booking);
        _eventRepoMock.Setup(x => x.GetByIdAsync(booking.EventId)).ReturnsAsync(@event);

        // Act
        await _sut.CancelBookingAsync(1);

        // Assert
        Assert.Equal(BookingStatus.Cancelada, booking.Status);
        _bookingRepoMock.Verify(x => x.UpdateAsync(booking), Times.Once);
        _bookingRepoMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }
    [Fact]
    public async Task CreateBookingAsync_LateReservation_ThrowsDomainException()
    {
        // Arrange (Evento en menos de 1 hora)
        var dto = new CreateBookingDto { EventId = 1, NumberOfTickets = 2, BuyerName = "Juan", BuyerEmail = "juan@test.com" };
        var startDate = DateTime.UtcNow.AddMinutes(30);
        var @event = new Event("Test", "Desc", 1, 100, startDate, startDate.AddHours(2), 50, EventType.Conferencia);
        
        _eventRepoMock.Setup(x => x.GetByIdAsync(dto.EventId)).ReturnsAsync(@event);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<DomainException>(() => _sut.CreateBookingAsync(dto));
        Assert.Equal(GlobalConstants.ErrorMessages.LateReservation, ex.Message);
    }

    [Fact]
    public async Task CreateBookingAsync_ShortNoticeExceedsMaxTickets_ThrowsDomainException()
    {
        // Arrange (Evento en 10 horas, intentando reservar 6 entradas)
        var dto = new CreateBookingDto { EventId = 1, NumberOfTickets = 6, BuyerName = "Juan", BuyerEmail = "juan@test.com" };
        var startDate = DateTime.UtcNow.AddHours(10);
        var @event = new Event("Test", "Desc", 1, 100, startDate, startDate.AddHours(2), 50, EventType.Conferencia);
        
        _eventRepoMock.Setup(x => x.GetByIdAsync(dto.EventId)).ReturnsAsync(@event);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<DomainException>(() => _sut.CreateBookingAsync(dto));
        Assert.Contains("solo se pueden reservar", ex.Message);
    }

    [Fact]
    public async Task CreateBookingAsync_HighPriceLimitExceeded_ThrowsDomainException()
    {
        // Arrange (Evento VIP > $100, intentando reservar 11 entradas)
        var dto = new CreateBookingDto { EventId = 1, NumberOfTickets = 11, BuyerName = "Juan", BuyerEmail = "juan@test.com" };
        var startDate = DateTime.UtcNow.AddDays(5);
        var @event = new Event("VIP Event", "Desc", 1, 100, startDate, startDate.AddHours(2), 150, EventType.Concierto);
        
        _eventRepoMock.Setup(x => x.GetByIdAsync(dto.EventId)).ReturnsAsync(@event);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<DomainException>(() => _sut.CreateBookingAsync(dto));
        Assert.Equal(GlobalConstants.ErrorMessages.ExceedsTransactionLimit, ex.Message);
    }
}
