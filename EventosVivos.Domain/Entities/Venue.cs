namespace EventosVivos.Domain.Entities;

public class Venue
{
    public int Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public int Capacity { get; private set; }
    public string City { get; private set; } = string.Empty;

    protected Venue() { } 

    public Venue(int id, string name, int capacity, string city)
    {
        Id = id;
        Name = name;
        Capacity = capacity;
        City = city;
    }
}
