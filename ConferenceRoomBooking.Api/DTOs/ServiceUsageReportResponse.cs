namespace ConferenceRoomBooking.Api.DTOs;

public class ServiceUsageReportResponse
{
    public int ServiceId { get; set; }

    public string ServiceName { get; set; } = string.Empty;

    public int UsageCount { get; set; }

    public decimal TotalRevenue { get; set; }
}