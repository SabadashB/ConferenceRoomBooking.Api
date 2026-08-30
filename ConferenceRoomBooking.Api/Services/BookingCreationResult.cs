using ConferenceRoomBooking.Api.DTOs;

namespace ConferenceRoomBooking.Api.Services;

public enum BookingCreationError
{
    None,
    RoomNotFound,
    BookingConflict,
    ServiceNotAvailable
}

public class BookingCreationResult
{
    public BookingResponse? Booking { get; init; }

    public BookingCreationError Error { get; init; }

    public bool Success => Error == BookingCreationError.None;
}