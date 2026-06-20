namespace EventosVivos.Domain.Enums;

public enum EventType
{
    Conferencia = 1,
    Taller = 2,
    Concierto = 3
}

public enum EventStatus
{
    Activo = 1,
    Completado = 2,
    Cancelado = 3
}

public enum BookingStatus
{
    PendientePago = 1,
    Confirmada = 2,
    Cancelada = 3
}
