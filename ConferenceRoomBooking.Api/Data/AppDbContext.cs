using ConferenceRoomBooking.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ConferenceRoomBooking.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Room> Rooms => Set<Room>();

    public DbSet<Service> Services => Set<Service>();

    public DbSet<Booking> Bookings => Set<Booking>();

    public DbSet<BookingService> BookingServices => Set<BookingService>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<BookingService>()
            .HasKey(x => new { x.BookingId, x.ServiceId });

        modelBuilder.Entity<BookingService>()
            .HasOne(x => x.Booking)
            .WithMany(x => x.BookingServices)
            .HasForeignKey(x => x.BookingId);

        modelBuilder.Entity<BookingService>()
            .HasOne(x => x.Service)
            .WithMany()
            .HasForeignKey(x => x.ServiceId);

        modelBuilder.Entity<Booking>()
            .HasOne(x => x.Room)
            .WithMany(x => x.Bookings)
            .HasForeignKey(x => x.RoomId);

        modelBuilder.Entity<Room>()
            .Property(x => x.HourlyRate)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Service>()
            .Property(x => x.Price)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Booking>()
            .Property(x => x.TotalPrice)
            .HasPrecision(18, 2);

        modelBuilder.Entity<BookingService>()
            .Property(x => x.Price)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Booking>()
            .Property(x => x.StartTime)
            .HasColumnType("timestamp with time zone");

        modelBuilder.Entity<Booking>()
            .Property(x => x.EndTime)
            .HasColumnType("timestamp with time zone");

        modelBuilder.Entity<RoomService>()
    .HasKey(x => new { x.RoomId, x.ServiceId });

        modelBuilder.Entity<RoomService>()
            .HasOne(x => x.Room)
            .WithMany(x => x.RoomServices)
            .HasForeignKey(x => x.RoomId);

        modelBuilder.Entity<RoomService>()
            .HasOne(x => x.Service)
            .WithMany(x => x.RoomServices)
            .HasForeignKey(x => x.ServiceId);

        modelBuilder.Entity<Room>().HasData(
            new Room
            {
                Id = 1,
                Name = "Зал A",
                Capacity = 50,
                HourlyRate = 2000,
                IsDeleted = false
            },
            new Room
            {
                Id = 2,
                Name = "Зал B",
                Capacity = 100,
                HourlyRate = 3500,
                IsDeleted = false
            },
            new Room
            {
                Id = 3,
                Name = "Зал C",
                Capacity = 30,
                HourlyRate = 1500,
                IsDeleted = false
        });

        modelBuilder.Entity<Service>().HasData(
            new Service
            {
                Id = 1,
                Name = "Проєктор",
                Price = 500
            },
            new Service
            {
                Id = 2,
                Name = "Wi-Fi",
                Price = 300
            },
            new Service
            {
                Id = 3,
                Name = "Звук",
                Price = 700
        });

        modelBuilder.Entity<RoomService>().HasData(
            // Зал A
            new RoomService
            {
                RoomId = 1,
                ServiceId = 1
            },
            new RoomService
            {
                RoomId = 1,
                ServiceId = 2
            },
            new RoomService
            {
                RoomId = 1,
                ServiceId = 3
            },

            // Зал B
            new RoomService
            {
                RoomId = 2,
                ServiceId = 1
            },
            new RoomService
            {
                RoomId = 2,
                ServiceId = 2
            },
            new RoomService
            {
                RoomId = 2,
                ServiceId = 3
            },

            // Зал C
            new RoomService
            {
                RoomId = 3,
                ServiceId = 1
            },
            new RoomService
            {
                RoomId = 3,
                ServiceId = 2
            },
            new RoomService
            {
                RoomId = 3,
                ServiceId = 3
        });
    }

}
