using System;
using EventosVivos.Domain.Enums;
using EventosVivos.Domain.Constants;
using EventosVivos.Domain.Exceptions;

namespace EventosVivos.Domain.Entities;

public class Booking
{
    public int Id { get; private set; }
    public int EventId { get; private set; }
    public Event Event { get; private set; } = null!;
    public int Quantity { get; private set; }
    public string BuyerName { get; private set; } = string.Empty;
    public string BuyerEmail { get; private set; } = string.Empty;
    public BookingStatus Status { get; private set; }
    public string? ReservationCode { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? CancelledAt { get; private set; }

    protected Booking() { }

    public Booking(int eventId, int quantity, string buyerName, string buyerEmail, Event @event)
    {
        if (quantity < 1)
            throw new DomainException("La cantidad debe ser 1 o más.");
            
        if (string.IsNullOrWhiteSpace(buyerName))
            throw new DomainException("El nombre del comprador es obligatorio.");
            
        if (string.IsNullOrWhiteSpace(buyerEmail) || !buyerEmail.Contains("@"))
            throw new DomainException("El email del comprador es inválido.");

        // Reglas de negocio vinculadas al evento
        TimeSpan timeUntilEvent = @event.StartDate - DateTime.UtcNow;

        // RN-04: Restricción de reserva tardía
        if (timeUntilEvent.TotalHours < GlobalConstants.EventLimits.LateReservationHours)
        {
            throw new DomainException(GlobalConstants.ErrorMessages.LateReservation);
        }

        // Regla: Si el evento tiene menos de 24 horas para iniciar, máximo 5 entradas
        if (timeUntilEvent.TotalHours < GlobalConstants.EventLimits.ShortNoticeHours && quantity > GlobalConstants.EventLimits.ShortNoticeMaxTickets)
        {
            throw new DomainException($"Para eventos próximos (menos de {GlobalConstants.EventLimits.ShortNoticeHours}h), solo se pueden reservar {GlobalConstants.EventLimits.ShortNoticeMaxTickets} entradas.");
        }

        // RN-05: Eventos con precio > $100 limitan a máximo 10 entradas
        if (@event.Price > GlobalConstants.EventLimits.HighPriceThreshold && quantity > GlobalConstants.EventLimits.HighPriceMaxTickets)
        {
            throw new DomainException(GlobalConstants.ErrorMessages.ExceedsTransactionLimit);
        }

        EventId = eventId;
        Quantity = quantity;
        BuyerName = buyerName;
        BuyerEmail = buyerEmail;
        Status = BookingStatus.PendientePago;
        CreatedAt = DateTime.UtcNow;
        ReservationCode = null;
    }

    public void ConfirmPayment()
    {
        if (Status == BookingStatus.Confirmada)
            throw new DomainException(GlobalConstants.ErrorMessages.BookingAlreadyConfirmed);
            
        if (Status == BookingStatus.Cancelada)
            throw new DomainException(GlobalConstants.ErrorMessages.BookingAlreadyCancelled);

        Status = BookingStatus.Confirmada;
        ReservationCode = $"{GlobalConstants.Prefix.BookingCode}{new Random().Next(100000, 999999)}";
    }

    public void Cancel(Event @event)
    {
        if (Status == BookingStatus.Cancelada)
            throw new DomainException(GlobalConstants.ErrorMessages.BookingAlreadyCancelled);

        // RN-07: Cancelación con penalización. Si es confirmada y faltan < 48h, se marca cancelada pero se considera "perdida" (se registra la fecha)
        // La lógica de liberar o no entradas se hace contando las reservas canceladas con penalidad o las confirmadas en el Repositorio.
        
        Status = BookingStatus.Cancelada;
        CancelledAt = DateTime.UtcNow;
    }
    
    // Auxiliar para saber si la cancelación tiene penalidad (no libera cupo)
    public bool IsPenaltyCancellation(Event @event)
    {
        if (Status != BookingStatus.Cancelada || !CancelledAt.HasValue) return false;
        
        TimeSpan timeUntilEvent = @event.StartDate - CancelledAt.Value;
        return timeUntilEvent.TotalHours < GlobalConstants.EventLimits.CancellationPenaltyHours;
    }
}
