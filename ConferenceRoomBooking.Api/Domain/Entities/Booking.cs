namespace ConferenceRoomBooking.Api.Domain.Entities;

public class Booking
{
    public int Id { get; set; }

    public int RoomId { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public decimal TotalPrice { get; set; }

    public Room Room { get; set; } = null!;

    public ICollection<BookingService> BookingServices { get; set; }
        = new List<BookingService>();
}