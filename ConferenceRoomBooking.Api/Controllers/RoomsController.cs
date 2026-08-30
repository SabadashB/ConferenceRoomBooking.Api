using ConferenceRoomBooking.Api.DTOs;
using ConferenceRoomBooking.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace ConferenceRoomBooking.Api.Controllers;

/// <summary>
/// Управління конференц-залами.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class RoomsController : ControllerBase
{
    private readonly RoomService _roomService;

    public RoomsController(RoomService roomService)
    {
        _roomService = roomService;
    }

    /// <summary>
    /// Отримати список всіх активних конференц-зал.
    /// </summary>
    /// <returns>Список залів і доступних послуг.</returns>
    /// <response code="200">Список залів успішно отримано.</response>
    [ProducesResponseType(
        typeof(List<AvailableRoomResponse>),
        StatusCodes.Status200OK)]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var rooms = await _roomService.GetAllAsync();

        return Ok(rooms);
    }

    /// <summary>
    /// Знайти вільні зали на вказаний інтервал часу.
    /// </summary>
    /// <param name="request">
    /// Час початку, час закінчення, необхідні місткість.
    /// </param>
    /// <returns>Список доступних залів.</returns>
    /// <response code="200">Зали знайдені.</response>
    /// <response code="400">
    /// Невірний діапазон часу або місткість.
    /// </response>
    [ProducesResponseType(
        typeof(List<AvailableRoomResponse>),
        StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpGet("available")]
    public async Task<IActionResult> GetAvailable(
        [FromQuery] AvailableRoomsRequest request)
    {
        if (request.StartTime >= request.EndTime)
            return BadRequest(
                "Час початку має бути раніше часу закінчення.");

        if (request.Capacity <= 0)
            return BadRequest(
                "Місткість має бути більше 0.");

        var startTime = DateTime.SpecifyKind(
            request.StartTime,
            DateTimeKind.Utc);

        var endTime = DateTime.SpecifyKind(
            request.EndTime,
            DateTimeKind.Utc);

        var rooms = await _roomService.GetAvailableAsync(
            startTime,
            endTime,
            request.Capacity);

        return Ok(rooms);
    }

    /// <summary>
    /// Створити новий конференц-зал.
    /// </summary>
    /// <param name="request">Дані нового залу.</param>
    /// <returns>Створений зал.</returns>
    /// <response code="201">Зал успішно створено.</response>
    /// <response code="400">Одна або декілька вказаних послуг не існують.</response>
    [ProducesResponseType(
        typeof(AvailableRoomResponse),
        StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPost]
    public async Task<IActionResult> Create(
        CreateRoomRequest request)
    {
        var room = await _roomService.CreateAsync(request);

        if (room == null)
            return BadRequest(
                "Одна або декілька вказаних послуг не існують.");

        return CreatedAtAction(
            nameof(GetAll),
            null,
            room);
    }

    /// <summary>
    /// Оновити дані конференц-залу.
    /// </summary>
    /// <param name="id">Ідентифікатор залу.</param>
    /// <param name="request">Оновлені дані залу.</param>
    /// <returns>Оновлений зал.</returns>
    /// <response code="200">Зал успішно оновлено.</response>
    /// <response code="400">Одна або декілька вказаних послуг не існують.</response>
    /// <response code="404">Зал не знайдено.</response>
    [ProducesResponseType(
        typeof(AvailableRoomResponse),
        StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        int id,
        UpdateRoomRequest request)
    {
        var result = await _roomService.UpdateAsync(id, request);

        if (!result.Success)
        {
            return result.Error switch
            {
                RoomUpdateError.RoomNotFound =>
                    NotFound("Зал не знайдено."),

                RoomUpdateError.ServiceNotFound =>
                    BadRequest("Одна або декілька вказаних послуг не існують."),

                _ => StatusCode(500, "Невідома помилка.")
            };
        }

        return Ok(result.Room);
    }

    /// <summary>
    /// Видалити конференц-зал.
    /// </summary>
    /// <param name="id">Ідентифікатор залу.</param>
    /// <returns>Результат видалення.</returns>
    /// <response code="200">Зал успішно видалено.</response>
    /// <response code="404">Зал не знайдено.</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _roomService.DeleteAsync(id);

        if (!deleted)
            return NotFound("Зал не знайдений.");

        return Ok(new
        {
            message = "Зал успішно видалений.",
            roomId = id
        });
    }
}
