using System.Threading.Tasks;
using EventosVivos.Api.Controllers.v1;
using EventosVivos.Application.DTOs;
using EventosVivos.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace EventosVivos.Tests.Api;

public class BookingsControllerTests
{
    private readonly Mock<IBookingService> _bookingServiceMock;
    private readonly BookingsController _sut;

    public BookingsControllerTests()
    {
        _bookingServiceMock = new Mock<IBookingService>();
        _sut = new BookingsController(_bookingServiceMock.Object);
    }

    [Fact]
    public async Task CreateBooking_Valid_ReturnsOkResult()
    {
        // Arrange
        var dto = new CreateBookingDto();
        var resultDto = new BookingDto { Id = 1, ReservationCode = "EV-123456" };
        
        _bookingServiceMock.Setup(x => x.CreateBookingAsync(dto)).ReturnsAsync(resultDto);

        // Act
        var result = await _sut.CreateBooking(dto);

        // Assert
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(_sut.GetBookingById), createdAtActionResult.ActionName);
        var returnValue = Assert.IsType<BookingDto>(createdAtActionResult.Value);
        Assert.Equal(1, returnValue.Id);
    }

    [Fact]
    public async Task CreateBooking_DomainException_ReturnsBadRequest()
    {
        // Arrange
        var dto = new CreateBookingDto();
        _bookingServiceMock.Setup(x => x.CreateBookingAsync(dto)).ThrowsAsync(new EventosVivos.Domain.Exceptions.DomainException("Error test"));

        // Act
        var result = await _sut.CreateBooking(dto);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Contains("Error test", badRequestResult.Value?.ToString());
    }

    [Fact]
    public async Task ConfirmPayment_Valid_ReturnsOkResult()
    {
        // Arrange
        _bookingServiceMock.Setup(x => x.ConfirmPaymentAsync(1)).Returns(Task.CompletedTask);

        // Act
        var result = await _sut.ConfirmPayment(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Contains("Pago confirmado", okResult.Value?.ToString());
    }

    [Fact]
    public async Task CancelBooking_Valid_ReturnsOkResult()
    {
        // Arrange
        _bookingServiceMock.Setup(x => x.CancelBookingAsync(1)).Returns(Task.CompletedTask);

        // Act
        var result = await _sut.CancelBooking(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Contains("Reserva cancelada", okResult.Value?.ToString());
    }

    [Fact]
    public async Task GetBookings_ReturnsOkResult()
    {
        // Arrange
        var pagedResult = new PagedResultDto<BookingDto>();
        _bookingServiceMock.Setup(x => x.GetBookingsAsync(It.IsAny<string?>(), It.IsAny<string?>(), 1, 10))
            .ReturnsAsync(pagedResult);

        // Act
        var result = await _sut.GetBookings(null, null, 1, 10);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(pagedResult, okResult.Value);
    }
}
