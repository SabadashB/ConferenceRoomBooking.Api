using ConferenceRoomBooking.Api.Data;
using ConferenceRoomBooking.Api.Domain.Entities;
using ConferenceRoomBooking.Api.DTOs;
using Microsoft.EntityFrameworkCore;

namespace ConferenceRoomBooking.Api.Services;

public class BookingService
{
    private readonly AppDbContext _context;
    private readonly BookingAvailabilityService _availabilityService;
    private readonly PricingService _pricingService;

    public BookingService(
        AppDbContext context,
        BookingAvailabilityService availabilityService,
        PricingService pricingService)
    {
        _context = context;
        _availabilityService = availabilityService;
        _pricingService = pricingService;
    }

    public async Task<BookingCreationResult> CreateAsync(
        CreateBookingRequest request)
    {
        var startTime = DateTime.SpecifyKind(
            request.StartTime,
            DateTimeKind.Utc);

        var duration = TimeSpan.FromHours(
            request.DurationHours);

        var endTime = startTime.Add(duration);

        var room = await _availabilityService.GetRoomAsync(
            request.RoomId);

        if (room == null)
        {
            return new BookingCreationResult
            {
                Error = BookingCreationError.RoomNotFound
            };
        }

        var hasConflict = await _availabilityService.HasConflictAsync(
            request.RoomId,
            startTime,
            endTime);

        if (hasConflict)
        {
            return new BookingCreationResult
            {
                Error = BookingCreationError.BookingConflict
            };
        }

        var serviceIds = request.ServiceIds
            .Distinct()
            .ToList();

        var selectedServices = room.RoomServices
            .Where(roomService =>
                serviceIds.Contains(roomService.ServiceId))
            .Select(roomService => roomService.Service)
            .ToList();

        if (selectedServices.Count != serviceIds.Count)
        {
            return new BookingCreationResult
            {
                Error = BookingCreationError.ServiceNotAvailable
            };
        }

        var roomPrice = _pricingService.CalculateRoomPrice(
            room,
            startTime,
            endTime);

        var servicesPrice = selectedServices
            .Sum(service => service.Price);

        var totalPrice = roomPrice + servicesPrice;

        var booking = new Booking
        {
            RoomId = room.Id,
            StartTime = startTime,
            EndTime = endTime,
            TotalPrice = totalPrice
        };

        foreach (var service in selectedServices)
        {
            booking.BookingServices.Add(
                new Domain.Entities.BookingService
                {
                    ServiceId = service.Id,
                    Price = service.Price
                });
        }

        _context.Bookings.Add(booking);

        await _context.SaveChangesAsync();

        return new BookingCreationResult
        {
            Error = BookingCreationError.None,
            Booking = new BookingResponse
            {
                Id = booking.Id,
                RoomId = booking.RoomId,
                StartTime = booking.StartTime,
                EndTime = booking.EndTime,
                TotalPrice = booking.TotalPrice,
                Services = selectedServices
                    .Select(service => new ServiceResponse
                    {
                        Id = service.Id,
                        Name = service.Name,
                        Price = service.Price
                    })
                    .ToList()
            }
        };
    }
}