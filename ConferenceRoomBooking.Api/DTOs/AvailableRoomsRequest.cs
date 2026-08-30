using System.ComponentModel.DataAnnotations;

namespace ConferenceRoomBooking.Api.DTOs;

public class AvailableRoomsRequest : IValidatableObject
{
    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    [Range(1, int.MaxValue)]
    public int Capacity { get; set; }

    public IEnumerable<ValidationResult> Validate(
        ValidationContext validationContext)
    {
        if (StartTime >= EndTime)
        {
            yield return new ValidationResult(
                "Час початку має бути раніше часу закінчення.",
                new[] { nameof(StartTime), nameof(EndTime) });
        }
    }
}