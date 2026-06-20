using System;
using System.Threading.Tasks;
using EventosVivos.Api.Controllers.v1;
using EventosVivos.Application.DTOs;
using EventosVivos.Application.Interfaces;
using EventosVivos.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace EventosVivos.Tests.Api;

public class EventsControllerTests
{
    private readonly Mock<IEventService> _eventServiceMock;
    private readonly EventsController _sut;

    public EventsControllerTests()
    {
        _eventServiceMock = new Mock<IEventService>();
        _sut = new EventsController(_eventServiceMock.Object);
    }

    [Fact]
    public async Task CreateEvent_Valid_ReturnsCreatedAtAction()
    {
        // Arrange
        var dto = new CreateEventDto();
        var resultDto = new EventDto { Id = 1, Title = "Test" };
        
        _eventServiceMock.Setup(x => x.CreateEventAsync(dto)).ReturnsAsync(resultDto);

        // Act
        var result = await _sut.CreateEvent(dto);

        // Assert
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(_sut.GetEventById), createdAtActionResult.ActionName);
        var returnValue = Assert.IsType<EventDto>(createdAtActionResult.Value);
        Assert.Equal(1, returnValue.Id);
    }

    [Fact]
    public async Task CreateEvent_DomainException_ReturnsBadRequest()
    {
        // Arrange
        var dto = new CreateEventDto();
        _eventServiceMock.Setup(x => x.CreateEventAsync(dto)).ThrowsAsync(new EventosVivos.Domain.Exceptions.DomainException("Error test"));

        // Act
        var result = await _sut.CreateEvent(dto);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Contains("Error test", badRequestResult.Value?.ToString());
    }

    [Fact]
    public async Task GetEvents_ReturnsOkResult()
    {
        // Arrange
        var pagedResult = new PagedResultDto<EventDto>();
        _eventServiceMock.Setup(x => x.GetEventsAsync(It.IsAny<EventType?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<int?>(), It.IsAny<EventStatus?>(), It.IsAny<string?>(), 1, 10))
            .ReturnsAsync(pagedResult);

        // Act
        var result = await _sut.GetEvents(null, null, null, null, null, null, 1, 10);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(pagedResult, okResult.Value);
    }

    [Fact]
    public async Task GetOccupancyReport_ReturnsOkResult()
    {
        // Arrange
        var reportDto = new OccupancyReportDto { EventId = 1 };
        _eventServiceMock.Setup(x => x.GetOccupancyReportAsync(1)).ReturnsAsync(reportDto);

        // Act
        var result = await _sut.GetOccupancyReport(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(reportDto, okResult.Value);
    }

    [Fact]
    public async Task GetOccupancyReport_NotFound_ReturnsNotFoundResult()
    {
        // Arrange
        _eventServiceMock.Setup(x => x.GetOccupancyReportAsync(1)).ReturnsAsync((OccupancyReportDto?)null);

        // Act
        var result = await _sut.GetOccupancyReport(1);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }
}
