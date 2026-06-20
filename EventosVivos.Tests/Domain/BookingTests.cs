using System;
using EventosVivos.Domain.Constants;
using EventosVivos.Domain.Entities;
using EventosVivos.Domain.Enums;
using EventosVivos.Domain.Exceptions;
using Xunit;

namespace EventosVivos.Tests.Domain;

public class BookingTests
{
    private Event CreateValidEvent(int daysAhead = 5, decimal price = 50)
    {
        var startDate = DateTime.UtcNow.AddDays(daysAhead).Date.AddHours(14);
        if (startDate.DayOfWeek == DayOfWeek.Saturday || startDate.DayOfWeek == DayOfWeek.Sunday)
            startDate = startDate.AddDays(2);
            
        return new Event("Concierto Test", "Description Test", 1, 100, startDate, startDate.AddHours(2), price, EventType.Concierto);
    }

    [Fact]
    public void CreateBooking_LateReservation_ShouldThrowException_RN04()
    {
        // Arrange
        // Crear un evento pero simular que falta muy poco usando un hack de Reflection o ajustando StartDate (Para el test, lo creamos con 2 horas pero luego le "cambiamos" la fecha)
        // Para respetar la inmutabilidad en tests, creamos un evento "valido" pero que al evaluar contra UtcNow falla por poco.
        // Dado que Event valida StartDate > UtcNow, el mínimo válido es UtcNow + 1ms.
        var startDate = DateTime.UtcNow.AddMinutes(30); // Faltan 30 minutos (Menos de 1 hora)
        
        var @event = new Event("Test Event", "Desc Test Test", 1, 100, startDate, startDate.AddHours(2), 50, EventType.Taller);
        
        // Act & Assert
        var ex = Assert.Throws<DomainException>(() => new Booking(1, 2, "Juan", "juan@test.com", @event));
        Assert.Equal(GlobalConstants.ErrorMessages.LateReservation, ex.Message);
    }

    [Fact]
    public void CreateBooking_HighPrice_Limit10Tickets_ShouldThrowException_RN05()
    {
        // Arrange
        var @event = CreateValidEvent(daysAhead: 5, price: 150); // Precio > 100
        
        // Act & Assert
        var ex = Assert.Throws<DomainException>(() => new Booking(1, 15, "Juan", "juan@test.com", @event)); // Pide 15
        Assert.Equal(GlobalConstants.ErrorMessages.ExceedsTransactionLimit, ex.Message);
    }
    
    [Fact]
    public void CancelBooking_Penalty_ShouldMarkAsPenalty_RN07()
    {
        // Arrange
        var startDate = DateTime.UtcNow.AddHours(24);
        var @event = new Event("Concierto Test", "Description Test", 1, 100, startDate, startDate.AddHours(2), 50, EventType.Concierto);
        var booking = new Booking(1, 2, "Juan", "juan@test.com", @event);
        booking.ConfirmPayment();
        
        // Act
        booking.Cancel(@event); // Se cancela a menos de 48h del evento
        
        // Assert
        Assert.Equal(BookingStatus.Cancelada, booking.Status);
        Assert.True(booking.IsPenaltyCancellation(@event));
    }
}
