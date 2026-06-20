using System;

namespace EventosVivos.Application.DTOs;

public class VenueDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public string City { get; set; } = string.Empty;
}
