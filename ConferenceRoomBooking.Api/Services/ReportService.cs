using ConferenceRoomBooking.Api.Data;
using ConferenceRoomBooking.Api.DTOs;
using Microsoft.EntityFrameworkCore;

namespace ConferenceRoomBooking.Api.Services;

public class ReportService
{
    private readonly AppDbContext _context;

    public ReportService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<RevenueReportResponse> GetRevenueAsync(
        DateTime from,
        DateTime to)
    {
        from = DateTime.SpecifyKind(from, DateTimeKind.Utc);
        to = DateTime.SpecifyKind(to, DateTimeKind.Utc);

        var bookings = _context.Bookings
            .Where(booking =>
                booking.StartTime >= from &&
                booking.StartTime <= to);

        return new RevenueReportResponse
        {
            From = from,
            To = to,
            TotalBookings = await bookings.CountAsync(),
            TotalRevenue = await bookings.SumAsync(
                booking => booking.TotalPrice)
        };
    }

    public async Task<List<RoomRevenueReportResponse>> GetRoomRevenueAsync(
        DateTime from,
        DateTime to)
    {
        from = DateTime.SpecifyKind(from, DateTimeKind.Utc);
        to = DateTime.SpecifyKind(to, DateTimeKind.Utc);

        return await _context.Bookings
            .Where(booking =>
                booking.StartTime >= from &&
                booking.StartTime <= to)
            .GroupBy(booking => new
            {
                booking.RoomId,
                booking.Room.Name
            })
            .Select(group => new RoomRevenueReportResponse
            {
                RoomId = group.Key.RoomId,
                RoomName = group.Key.Name,
                BookingCount = group.Count(),
                TotalRevenue = group.Sum(booking => booking.TotalPrice)
            })
            .OrderByDescending(report => report.TotalRevenue)
            .ToListAsync();
    }

    public async Task<List<ServiceUsageReportResponse>> GetServiceUsageAsync(
        DateTime from,
        DateTime to)
    {
        from = DateTime.SpecifyKind(from, DateTimeKind.Utc);
        to = DateTime.SpecifyKind(to, DateTimeKind.Utc);

        return await _context.BookingServices
            .Where(bookingService =>
                bookingService.Booking.StartTime >= from &&
                bookingService.Booking.StartTime <= to)
            .GroupBy(bookingService => new
            {
                bookingService.ServiceId,
                bookingService.Service.Name
            })
            .Select(group => new ServiceUsageReportResponse
            {
                ServiceId = group.Key.ServiceId,
                ServiceName = group.Key.Name,
                UsageCount = group.Count(),
                TotalRevenue = group.Sum(bookingService => bookingService.Price)
            })
            .OrderByDescending(report => report.UsageCount)
            .ToListAsync();
    }
}