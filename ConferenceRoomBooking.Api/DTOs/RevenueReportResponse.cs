namespace ConferenceRoomBooking.Api.DTOs;

public class RevenueReportResponse
{
    public DateTime From { get; set; }

    public DateTime To { get; set; }

    public int TotalBookings { get; set; }

    public decimal TotalRevenue { get; set; }
}