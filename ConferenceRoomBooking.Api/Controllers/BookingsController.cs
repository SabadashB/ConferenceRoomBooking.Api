using ConferenceRoomBooking.Api.DTOs;
using ConferenceRoomBooking.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace ConferenceRoomBooking.Api.Controllers;

/// <summary>
/// Керує бронюваннями конференц-залів.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class BookingsController : ControllerBase
{
    private readonly BookingService _bookingService;

    public BookingsController(BookingService bookingService)
    {
        _bookingService = bookingService;
    }

    /// <summary>
    /// Створити бронювання конференц-залу.
    /// </summary>
    /// <param name="request">
    /// Зал, час початку, тривалість та вибрані послуги.
    /// </param>
    /// <returns>Створене бронювання з підсумковою вартістю.</returns>
    /// <response code="200">
    /// Бронювання успішно створено.
    /// </response>
    /// <response code="400">
    /// Вибрані послуги недоступні для цього залу.
    /// </response>
    /// <response code="404">
    /// Зал не знайдено.
    /// </response>
    /// <response code="409">
    /// Зал уже заброньований на вказаний час.
    /// </response>
    [ProducesResponseType(
        typeof(BookingResponse),
        StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [HttpPost]
    public async Task<IActionResult> Create(
        CreateBookingRequest request)
    {
        var result = await _bookingService.CreateAsync(request);

        if (!result.Success)
        {
            return result.Error switch
            {
                BookingCreationError.RoomNotFound =>
                    NotFound("Зал не знайдено."),

                BookingCreationError.BookingConflict =>
                    Conflict("Зал уже заброньовано на вказаний час."),

                BookingCreationError.ServiceNotAvailable =>
                    BadRequest(
                        "Одна або декілька вибраних послуг недоступні для цього залу."),

                _ => StatusCode(500, "Невідома помилка.")
            };
        }

        return Ok(result.Booking);
    }
}
