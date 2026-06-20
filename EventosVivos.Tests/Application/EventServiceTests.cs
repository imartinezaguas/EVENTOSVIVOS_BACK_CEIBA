using System;
using System.Collections.Generic;
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

public class EventServiceTests
{
    private readonly Mock<IEventRepository> _eventRepoMock;
    private readonly Mock<IVenueRepository> _venueRepoMock;
    private readonly Mock<IBookingRepository> _bookingRepoMock;
    private readonly EventService _sut;

    public EventServiceTests()
    {
        _eventRepoMock = new Mock<IEventRepository>();
        _venueRepoMock = new Mock<IVenueRepository>();
        _bookingRepoMock = new Mock<IBookingRepository>();

        _sut = new EventService(
            _eventRepoMock.Object,
            _venueRepoMock.Object,
            _bookingRepoMock.Object);
    }

    [Fact]
    public async Task CreateEventAsync_ValidData_ShouldReturnEventDto()
    {
        // Arrange
        var dto = new CreateEventDto
        {
            Title = "Test Event",
            Description = "A valid description for test",
            VenueId = 1,
            MaxCapacity = 50,
            StartDate = DateTime.UtcNow.AddDays(1).Date.AddHours(14),
            EndDate = DateTime.UtcNow.AddDays(1).Date.AddHours(18),
            Price = 100,
            Type = EventType.Conferencia
        };

        var venue = new Venue(1, "Test Venue", 100, "Ciudad");

        _venueRepoMock.Setup(x => x.GetByIdAsync(dto.VenueId))
            .ReturnsAsync(venue);

        _eventRepoMock.Setup(x => x.HasOverlappingEventsAsync(dto.VenueId, dto.StartDate, dto.EndDate))
            .ReturnsAsync(false);

        // Act
        var result = await _sut.CreateEventAsync(dto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(dto.Title, result.Title);
        _eventRepoMock.Verify(x => x.AddAsync(It.IsAny<Event>()), Times.Once);
        _eventRepoMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateEventAsync_CapacityExceeded_ShouldThrowDomainException_RN01()
    {
        // Arrange
        var dto = new CreateEventDto { VenueId = 1, MaxCapacity = 200 };
        var venue = new Venue(1, "Test Venue", 100, "Ciudad"); // Solo 100 max

        _venueRepoMock.Setup(x => x.GetByIdAsync(dto.VenueId)).ReturnsAsync(venue);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<DomainException>(() => _sut.CreateEventAsync(dto));
        Assert.Equal(GlobalConstants.ErrorMessages.CapacityExceeded, ex.Message);
    }

    [Fact]
    public async Task CreateEventAsync_OverlappingVenue_ShouldThrowDomainException_RN02()
    {
        // Arrange
        var dto = new CreateEventDto { VenueId = 1, MaxCapacity = 50, StartDate = DateTime.UtcNow.AddDays(1).Date.AddHours(14), EndDate = DateTime.UtcNow.AddDays(1).Date.AddHours(16) };
        var venue = new Venue(1, "Test Venue", 100, "Ciudad");

        _venueRepoMock.Setup(x => x.GetByIdAsync(dto.VenueId)).ReturnsAsync(venue);
        _eventRepoMock.Setup(x => x.HasOverlappingEventsAsync(dto.VenueId, dto.StartDate, dto.EndDate)).ReturnsAsync(true);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<DomainException>(() => _sut.CreateEventAsync(dto));
        Assert.Equal(GlobalConstants.ErrorMessages.VenueOverlap, ex.Message);
    }

    [Fact]
    public async Task GetEventsAsync_ReturnsPagedResult()
    {
        // Arrange
        var eventsList = new List<Event> 
        { 
            new Event("Event 1", "Valid Description 1", 1, 50, DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(1).AddHours(2), 50, EventType.Conferencia) 
        };
        
        _eventRepoMock.Setup(x => x.GetEventsAsync(null, null, null, null, null, null, 1, 10))
            .ReturnsAsync((eventsList, 1));

        // Act
        var result = await _sut.GetEventsAsync(null, null, null, null, null, null, 1, 10);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.TotalRecords);
        Assert.Single(result.Items);
    }

    [Fact]
    public async Task GetOccupancyReportAsync_ReturnsReport()
    {
        // Arrange
        var @event = new Event("Event 1", "Valid Description 1", 1, 100, DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(1).AddHours(2), 50, EventType.Conferencia);
        
        _eventRepoMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(@event);
        _bookingRepoMock.Setup(x => x.GetConfirmedAndPenaltyTicketsCountForEventAsync(1)).ReturnsAsync(30);

        // Act
        var result = await _sut.GetOccupancyReportAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(30, result.TotalConfirmedTickets);
        Assert.Equal(70, result.RemainingTickets);
        Assert.Equal(30.0, result.OccupancyPercentage);
        Assert.Equal(1500m, result.TotalRevenue);
    }
}
