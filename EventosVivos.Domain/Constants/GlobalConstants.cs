namespace EventosVivos.Domain.Constants;

public static class GlobalConstants
{
    public static class EventLimits
    {
        public const int MinTitleLength = 5;
        public const int MaxTitleLength = 100;
        public const int MinDescriptionLength = 10;
        public const int MaxDescriptionLength = 500;
        
        public const int ShortNoticeHours = 24;
        public const int ShortNoticeMaxTickets = 5;
        
        public const decimal HighPriceThreshold = 100m;
        public const int HighPriceMaxTickets = 10;
        
        public const int LateReservationHours = 1;
        public const int CancellationPenaltyHours = 48;
        
        public const int WeekendNightRestrictionHour = 22;
    }

    public static class ErrorMessages
    {
        public const string VenueNotFound = "El venue no existe.";
        public const string CapacityExceeded = "El evento no puede exceder la capacidad del venue.";
        public const string VenueOverlap = "Dos eventos activos no pueden compartir el mismo venue con horarios superpuestos.";
        public const string WeekendNightRestriction = "Eventos en fin de semana no pueden iniciar después de las 22:00.";
        public const string InvalidDates = "La fecha de fin debe ser posterior al inicio.";
        public const string InvalidStartDate = "La fecha y hora de inicio debe ser futura.";
        
        // Booking Errors
        public const string EventNotFound = "El evento no existe.";
        public const string NotEnoughTickets = "No hay suficientes entradas disponibles.";
        public const string LateReservation = "No se permiten reservas para eventos que inicien en menos de 1 hora.";
        public const string ExceedsTransactionLimit = "Límite de entradas excedido para esta transacción.";
        public const string BookingAlreadyConfirmed = "La reserva ya está confirmada.";
        public const string BookingAlreadyCancelled = "La reserva ya está cancelada.";
    }

    public static class Prefix
    {
        public const string BookingCode = "EV-";
    }

    public static class Pagination
    {
        public const int DefaultPageSize = 10;
        public const int MaxPageSize = 50;
    }
}
