namespace ConferenceRoomBooking.Api.DTOs;

public class RoomRevenueReportResponse
{
    public int RoomId { get; set; }

    public string RoomName { get; set; } = string.Empty;

    public int BookingCount { get; set; }

    public decimal TotalRevenue { get; set; }
}
