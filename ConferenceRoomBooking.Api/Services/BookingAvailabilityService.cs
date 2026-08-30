using ConferenceRoomBooking.Api.Data;
using ConferenceRoomBooking.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ConferenceRoomBooking.Api.Services;

public class BookingAvailabilityService
{
    private readonly AppDbContext _context;

    public BookingAvailabilityService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Room?> GetRoomAsync(int roomId)
    {
        return await _context.Rooms
            .Include(room => room.RoomServices)
                .ThenInclude(roomService => roomService.Service)
            .FirstOrDefaultAsync(room =>
                room.Id == roomId &&
                !room.IsDeleted);
    }

    public async Task<bool> HasConflictAsync(
        int roomId,
        DateTime startTime,
        DateTime endTime)
    {
        return await _context.Bookings
            .AnyAsync(booking =>
                booking.RoomId == roomId &&
                booking.StartTime < endTime &&
                booking.EndTime > startTime);
    }
}
