namespace ConferenceRoomBooking.Api.Domain.Entities;

public class Room
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public int Capacity { get; set; }

    public decimal HourlyRate { get; set; }

    public bool IsDeleted { get; set; }

    public ICollection<Booking> Bookings { get; set; }
        = new List<Booking>();
    public ICollection<RoomService> RoomServices { get; set; }
    = new List<RoomService>();
}
