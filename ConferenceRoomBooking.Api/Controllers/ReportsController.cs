using ConferenceRoomBooking.Api.DTOs;
using ConferenceRoomBooking.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace ConferenceRoomBooking.Api.Controllers;

/// <summary>
/// Звіти та аналітика щодо бронювань.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly ReportService _reportService;

    public ReportsController(ReportService reportService)
    {
        _reportService = reportService;
    }

    /// <summary>
    /// Отримати загальну виручку та кількість бронювань за період.
    /// </summary>
    /// <param name="from">Початок періоду.</param>
    /// <param name="to">Кінець періоду.</param>
    /// <response code="200">Звіт успішно сформовано.</response>
    /// <response code="400">Некоректний діапазон дат.</response>
    [ProducesResponseType(
        typeof(RevenueReportResponse),
        StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpGet("revenue")]
    public async Task<IActionResult> GetRevenue(
        [FromQuery] DateTime from,
        [FromQuery] DateTime to)
    {
        if (from > to)
            return BadRequest(
                "Початкова дата має бути раніше кінцевої.");

        var report = await _reportService.GetRevenueAsync(from, to);

        return Ok(report);
    }

    /// <summary>
    /// Отримати статистику бронювань та виручки за залами.
    /// </summary>
    /// <param name="from">Початок періоду.</param>
    /// <param name="to">Кінець періоду.</param>
    /// <response code="200">Звіт успішно сформовано.</response>
    /// <response code="400">Некоректний діапазон дат.</response>
    [ProducesResponseType(
        typeof(List<RoomRevenueReportResponse>),
        StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpGet("rooms")]
    public async Task<IActionResult> GetRoomRevenue(
        [FromQuery] DateTime from,
        [FromQuery] DateTime to)
    {
        if (from > to)
            return BadRequest(
                "Початкова дата має бути раніше кінцевої.");

        var report = await _reportService.GetRoomRevenueAsync(from, to);

        return Ok(report);
    }

    /// <summary>
    /// Отримати статистику використання та виручки за послугами.
    /// </summary>
    /// <param name="from">Початок періоду.</param>
    /// <param name="to">Кінець періоду.</param>
    /// <response code="200">Звіт успішно сформовано.</response>
    /// <response code="400">Некоректний діапазон дат.</response>
    [ProducesResponseType(
        typeof(List<ServiceUsageReportResponse>),
        StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpGet("services")]
    public async Task<IActionResult> GetServiceUsage(
        [FromQuery] DateTime from,
        [FromQuery] DateTime to)
    {
        if (from > to)
            return BadRequest(
                "Початкова дата має бути раніше кінцевої.");

        var report = await _reportService.GetServiceUsageAsync(from, to);

        return Ok(report);
    }
}