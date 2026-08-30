using System.ComponentModel.DataAnnotations;

namespace ConferenceRoomBooking.Api.DTOs;

public class CreateBookingRequest
{
    [Range(1, int.MaxValue)]
    public int RoomId { get; set; }

    public DateTime StartTime { get; set; }

    [Range(0.01, 24)]
    public double DurationHours { get; set; }

    public List<int> ServiceIds { get; set; } = new();
}