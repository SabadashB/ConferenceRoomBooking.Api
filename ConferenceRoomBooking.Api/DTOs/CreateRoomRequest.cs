using System.ComponentModel.DataAnnotations;

namespace ConferenceRoomBooking.Api.DTOs;

public class CreateRoomRequest
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int Capacity { get; set; }

    [Range(0.01, double.MaxValue)]
    public decimal HourlyRate { get; set; }

    public List<int> ServiceIds { get; set; } = new();
}