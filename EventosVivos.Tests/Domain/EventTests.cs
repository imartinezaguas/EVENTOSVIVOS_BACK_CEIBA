using System;
using EventosVivos.Domain.Constants;
using EventosVivos.Domain.Entities;
using EventosVivos.Domain.Enums;
using EventosVivos.Domain.Exceptions;
using Xunit;

namespace EventosVivos.Tests.Domain;

public class EventTests
{
    [Fact]
    public void CreateEvent_ValidData_ShouldCreateEvent()
    {
        // Arrange
        var title = "Conferencia Anual";
        var description = "Una gran conferencia";
        var startDate = DateTime.UtcNow.AddDays(5).Date.AddHours(10); // 10 AM (Valido fin de semana o semana)
        if (startDate.DayOfWeek == DayOfWeek.Saturday || startDate.DayOfWeek == DayOfWeek.Sunday)
            startDate = startDate.AddDays(2); // Asegurar que es día de semana para evitar conflictos con RN-03 en este test básico
            
        var endDate = startDate.AddHours(2);
        
        // Act
        var @event = new Event(title, description, 1, 100, startDate, endDate, 50, EventType.Conferencia);
        
        // Assert
        Assert.NotNull(@event);
        Assert.Equal(title, @event.Title);
        Assert.Equal(EventStatus.Activo, @event.Status);
    }

    [Fact]
    public void CreateEvent_WeekendNight_ShouldThrowDomainException_RN03()
    {
        // Arrange
        var title = "Concierto Nocturno";
        var description = "Concierto de rock";
        
        // Buscar el próximo sábado
        var startDate = DateTime.UtcNow.AddDays(1);
        while (startDate.DayOfWeek != DayOfWeek.Saturday)
        {
            startDate = startDate.AddDays(1);
        }
        startDate = new DateTime(startDate.Year, startDate.Month, startDate.Day, 23, 0, 0, DateTimeKind.Unspecified); // Sábado a las 11 PM
        var endDate = startDate.AddHours(2);
        
        // Act & Assert
        var ex = Assert.Throws<DomainException>(() => 
            new Event(title, description, 1, 100, startDate, endDate, 50, EventType.Concierto));
            
        Assert.Equal(GlobalConstants.ErrorMessages.WeekendNightRestriction, ex.Message);
    }
}
