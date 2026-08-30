using ConferenceRoomBooking.Api.Data;
using ConferenceRoomBooking.Api.DTOs;
using ConferenceRoomBooking.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ConferenceRoomBooking.Api.Services;

public class RoomService
{
    private readonly AppDbContext _context;

    public RoomService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<AvailableRoomResponse>> GetAllAsync()
    {
        return await _context.Rooms
            .Where(room => !room.IsDeleted)
            .Select(room => new AvailableRoomResponse
            {
                Id = room.Id,
                Name = room.Name,
                Capacity = room.Capacity,
                HourlyRate = room.HourlyRate,
                Services = room.RoomServices
                    .Select(roomService => new ServiceResponse
                    {
                        Id = roomService.Service.Id,
                        Name = roomService.Service.Name,
                        Price = roomService.Service.Price
                    })
                    .ToList()
            })
            .ToListAsync();
    }

    public async Task<AvailableRoomResponse?> GetByIdAsync(int id)
    {
        return await _context.Rooms
            .Where(room =>
                room.Id == id &&
                !room.IsDeleted)
            .Select(room => new AvailableRoomResponse
            {
                Id = room.Id,
                Name = room.Name,
                Capacity = room.Capacity,
                HourlyRate = room.HourlyRate,
                Services = room.RoomServices
                    .Select(roomService => new ServiceResponse
                    {
                        Id = roomService.Service.Id,
                        Name = roomService.Service.Name,
                        Price = roomService.Service.Price
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync();
    }

    public async Task<List<AvailableRoomResponse>> GetAvailableAsync(
        DateTime startTime,
        DateTime endTime,
        int capacity)
    {
        return await _context.Rooms
            .Where(room =>
                !room.IsDeleted &&
                room.Capacity >= capacity &&
                !room.Bookings.Any(booking =>
                    booking.StartTime < endTime &&
                    booking.EndTime > startTime))
            .Select(room => new AvailableRoomResponse
            {
                Id = room.Id,
                Name = room.Name,
                Capacity = room.Capacity,
                HourlyRate = room.HourlyRate,
                Services = room.RoomServices
                    .Select(roomService => new ServiceResponse
                    {
                        Id = roomService.Service.Id,
                        Name = roomService.Service.Name,
                        Price = roomService.Service.Price
                    })
                    .ToList()
            })
            .ToListAsync();
    }

    public async Task<AvailableRoomResponse?> CreateAsync(
        CreateRoomRequest request)
    {
        var serviceIds = request.ServiceIds
            .Distinct()
            .ToList();

        var services = await _context.Services
            .Where(service => serviceIds.Contains(service.Id))
            .ToListAsync();

        if (services.Count != serviceIds.Count)
            return null;

        var room = new Room
        {
            Name = request.Name,
            Capacity = request.Capacity,
            HourlyRate = request.HourlyRate,
            IsDeleted = false
        };

        foreach (var service in services)
        {
            room.RoomServices.Add(new Domain.Entities.RoomService
            {
                ServiceId = service.Id,
                Service = service
            });
        }

        _context.Rooms.Add(room);

        await _context.SaveChangesAsync();

        return await GetByIdAsync(room.Id);
    }

        public async Task<RoomUpdateResult> UpdateAsync(
    int id,
    UpdateRoomRequest request)
    {
        var room = await _context.Rooms
            .Include(room => room.RoomServices)
            .FirstOrDefaultAsync(room =>
                room.Id == id &&
                !room.IsDeleted);

        if (room == null)
        {
            return new RoomUpdateResult
            {
                Error = RoomUpdateError.RoomNotFound
            };
        }

        var serviceIds = request.ServiceIds
            .Distinct()
            .ToList();

        var services = await _context.Services
            .Where(service => serviceIds.Contains(service.Id))
            .ToListAsync();

        if (services.Count != serviceIds.Count)
        {
            return new RoomUpdateResult
            {
                Error = RoomUpdateError.ServiceNotFound
            };
        }

        room.Name = request.Name;
        room.Capacity = request.Capacity;
        room.HourlyRate = request.HourlyRate;

        room.RoomServices.Clear();

        foreach (var service in services)
        {
            room.RoomServices.Add(
                new Domain.Entities.RoomService
                {
                    RoomId = room.Id,
                    ServiceId = service.Id,
                    Service = service
                });
        }

        await _context.SaveChangesAsync();

        return new RoomUpdateResult
        {
            Error = RoomUpdateError.None,
            Room = await GetByIdAsync(room.Id)
        };
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var room = await _context.Rooms
            .FirstOrDefaultAsync(room =>
                room.Id == id &&
                !room.IsDeleted);

        if (room == null)
            return false;

        room.IsDeleted = true;

        await _context.SaveChangesAsync();

        return true;
    }
}