using ConferenceRoomBooking.Api.Domain.Entities;

namespace ConferenceRoomBooking.Api.Services;

public class PricingService
{
    public decimal CalculateRoomPrice(
        Room room,
        DateTime startTime,
        DateTime endTime)
    {
        decimal totalPrice = 0;
        var currentTime = startTime;

        while (currentTime < endTime)
        {
            var nextBoundary = GetNextPriceBoundary(currentTime);

            if (nextBoundary > endTime)
                nextBoundary = endTime;

            var duration = (decimal)(nextBoundary - currentTime).TotalHours;
            var multiplier = GetPriceMultiplier(currentTime);

            totalPrice += room.HourlyRate * duration * multiplier;

            currentTime = nextBoundary;
        }

        return totalPrice;
    }

    private static decimal GetPriceMultiplier(DateTime time)
    {
        var currentTime = time.TimeOfDay;

        if (currentTime >= TimeSpan.FromHours(12) &&
            currentTime < TimeSpan.FromHours(14))
        {
            return 1.15m;
        }

        if (currentTime >= TimeSpan.FromHours(18) &&
            currentTime < TimeSpan.FromHours(23))
        {
            return 0.80m;
        }

        if (currentTime >= TimeSpan.FromHours(6) &&
            currentTime < TimeSpan.FromHours(9))
        {
            return 0.90m;
        }

        return 1.00m;
    }

    private static DateTime GetNextPriceBoundary(DateTime currentTime)
    {
        var date = currentTime.Date;
        var time = currentTime.TimeOfDay;

        var boundaries = new[]
        {
            new TimeSpan(6, 0, 0),
            new TimeSpan(9, 0, 0),
            new TimeSpan(12, 0, 0),
            new TimeSpan(14, 0, 0),
            new TimeSpan(18, 0, 0),
            new TimeSpan(23, 0, 0)
        };

        foreach (var boundary in boundaries)
        {
            if (boundary > time)
                return date.Add(boundary);
        }

        return date.AddDays(1).Add(boundaries[0]);
    }
}
