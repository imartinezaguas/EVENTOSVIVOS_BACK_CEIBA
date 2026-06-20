using EventosVivos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventosVivos.Infrastructure.Data.Configurations;

public class EventConfiguration : IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Title).IsRequired().HasMaxLength(100);
        builder.Property(e => e.Description).IsRequired().HasMaxLength(500);
        builder.Property(e => e.Price).HasColumnType("decimal(18,2)");
        
        builder.HasOne(e => e.Venue)
               .WithMany()
               .HasForeignKey(e => e.VenueId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}

public class VenueConfiguration : IEntityTypeConfiguration<Venue>
{
    public void Configure(EntityTypeBuilder<Venue> builder)
    {
        builder.HasKey(v => v.Id);
        builder.Property(v => v.Name).IsRequired().HasMaxLength(200);
        builder.Property(v => v.City).IsRequired().HasMaxLength(100);
    }
}

public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.HasKey(b => b.Id);
        builder.Property(b => b.BuyerName).IsRequired().HasMaxLength(200);
        builder.Property(b => b.BuyerEmail).IsRequired().HasMaxLength(200);
        builder.Property(b => b.ReservationCode).HasMaxLength(50);
        
        builder.HasOne(b => b.Event)
               .WithMany()
               .HasForeignKey(b => b.EventId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
