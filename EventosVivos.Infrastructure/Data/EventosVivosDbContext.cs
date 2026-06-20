using EventosVivos.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EventosVivos.Infrastructure.Data;

public class EventosVivosDbContext : DbContext
{
    public EventosVivosDbContext(DbContextOptions<EventosVivosDbContext> options) : base(options)
    {
    }

    public DbSet<Venue> Venues { get; set; } = null!;
    public DbSet<Event> Events { get; set; } = null!;
    public DbSet<Booking> Bookings { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(EventosVivosDbContext).Assembly);
        
        // Seed initial venues data
        modelBuilder.Entity<Venue>().HasData(
            new Venue(1, "Auditorio Central", 200, "Bogotá"),
            new Venue(2, "Sala Norte", 50, "Bogotá"),
            new Venue(3, "Arena Sur", 500, "Medellín")
        );
    }
}
