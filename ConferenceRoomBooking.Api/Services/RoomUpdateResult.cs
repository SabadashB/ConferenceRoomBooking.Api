using ConferenceRoomBooking.Api.DTOs;

namespace ConferenceRoomBooking.Api.Services;

public enum RoomUpdateError
{
    None,
    RoomNotFound,
    ServiceNotFound
}

public class RoomUpdateResult
{
    public AvailableRoomResponse? Room { get; init; }

    public RoomUpdateError Error { get; init; }

    public bool Success => Error == RoomUpdateError.None;
}
