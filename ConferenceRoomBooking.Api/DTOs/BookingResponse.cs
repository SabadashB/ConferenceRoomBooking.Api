namespace ConferenceRoomBooking.Api.DTOs;

public class BookingResponse
{
    public int Id { get; set; }

    public int RoomId { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public decimal TotalPrice { get; set; }

    public List<ServiceResponse> Services { get; set; } = new();
}
