using System;
using EventosVivos.Domain.Enums;
using EventosVivos.Domain.Constants;
using EventosVivos.Domain.Exceptions;

namespace EventosVivos.Domain.Entities;

public class Event
{
    public int Id { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public int VenueId { get; private set; }
    public Venue Venue { get; private set; } = null!;
    public int MaxCapacity { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public decimal Price { get; private set; }
    public EventType Type { get; private set; }
    public EventStatus Status { get; private set; }

    protected Event() { }

    public Event(string title, string description, int venueId, int maxCapacity, 
        DateTime startDate, DateTime endDate, decimal price, EventType type)
    {
        ValidateCreation(title, description, startDate, endDate, price, maxCapacity);
        
        Title = title;
        Description = description;
        VenueId = venueId;
        MaxCapacity = maxCapacity;
        StartDate = startDate;
        EndDate = endDate;
        Price = price;
        Type = type;
        Status = EventStatus.Activo;
    }

    private void ValidateCreation(string title, string description, DateTime startDate, DateTime endDate, decimal price, int capacity)
    {
        if (string.IsNullOrWhiteSpace(title) || title.Length < GlobalConstants.EventLimits.MinTitleLength || title.Length > GlobalConstants.EventLimits.MaxTitleLength)
            throw new DomainException($"El título debe tener entre {GlobalConstants.EventLimits.MinTitleLength} y {GlobalConstants.EventLimits.MaxTitleLength} caracteres.");
            
        if (string.IsNullOrWhiteSpace(description) || description.Length < GlobalConstants.EventLimits.MinDescriptionLength || description.Length > GlobalConstants.EventLimits.MaxDescriptionLength)
            throw new DomainException($"La descripción debe tener entre {GlobalConstants.EventLimits.MinDescriptionLength} y {GlobalConstants.EventLimits.MaxDescriptionLength} caracteres.");
            
        if (startDate <= DateTime.UtcNow)
            throw new DomainException(GlobalConstants.ErrorMessages.InvalidStartDate);
            
        if (endDate <= startDate)
            throw new DomainException(GlobalConstants.ErrorMessages.InvalidDates);
            
        if (price < 0)
            throw new DomainException("El precio debe ser positivo.");
            
        if (capacity <= 0)
            throw new DomainException("La capacidad máxima debe ser mayor a cero.");

        // RN-03 Restricción de horario nocturno
        // Ajustamos la hora a hora local (Colombia UTC-5) para evaluar la regla de negocio
        var localStartDate = startDate.Kind == DateTimeKind.Utc ? startDate.AddHours(-5) : startDate;
        bool isWeekend = localStartDate.DayOfWeek == DayOfWeek.Saturday || localStartDate.DayOfWeek == DayOfWeek.Sunday;
        if (isWeekend && localStartDate.Hour >= GlobalConstants.EventLimits.WeekendNightRestrictionHour)
        {
            throw new DomainException(GlobalConstants.ErrorMessages.WeekendNightRestriction);
        }
    }

    public void MarkAsCompleted()
    {
        if (Status == EventStatus.Activo && DateTime.UtcNow > EndDate)
        {
            Status = EventStatus.Completado;
        }
    }

    public void Cancel()
    {
        Status = EventStatus.Cancelado;
    }
}
