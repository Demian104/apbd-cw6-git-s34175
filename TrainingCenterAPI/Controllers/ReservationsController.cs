using Microsoft.AspNetCore.Mvc;
using TrainingCenterAPI.Data;
using TrainingCenterAPI.Models;

namespace TrainingCenterAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReservationsController : ControllerBase
{
    // GET: /api/reservations?date=2026-05-10&status=confirmed&roomId=2
    [HttpGet]
    public ActionResult<IEnumerable<Reservation>> GetReservations(
        [FromQuery] DateTime? date,
        [FromQuery] string? status,
        [FromQuery] int? roomId)
    {
        var reservations = DataStore.Reservations.AsQueryable();

        if (date.HasValue)
            reservations = reservations.Where(r => r.Date.Date == date.Value.Date);

        if (!string.IsNullOrEmpty(status))
            reservations = reservations.Where(r => r.Status.Equals(status, StringComparison.OrdinalIgnoreCase));

        if (roomId.HasValue)
            reservations = reservations.Where(r => r.RoomId == roomId.Value);

        return Ok(reservations.ToList());
    }

    // GET: /api/reservations/{id}
    [HttpGet("{id}")]
    public ActionResult<Reservation> GetReservation(int id)
    {
        var reservation = DataStore.Reservations.FirstOrDefault(r => r.Id == id);
        if (reservation == null)
            return NotFound($"Reservation with ID {id} not found.");

        return Ok(reservation);
    }

    // POST: /api/reservations
    [HttpPost]
    public ActionResult<Reservation> CreateReservation([FromBody] Reservation reservation)
    {
        var room = DataStore.Rooms.FirstOrDefault(r => r.Id == reservation.RoomId);
        
        if (room == null)
            return NotFound($"Room with ID {reservation.RoomId} does not exist.");

        if (!room.IsActive)
            return BadRequest($"Room with ID {reservation.RoomId} is currently inactive.");

        var isConflict = DataStore.Reservations.Any(r =>
            r.RoomId == reservation.RoomId &&
            r.Date.Date == reservation.Date.Date &&
            r.StartTime < reservation.EndTime &&
            r.EndTime > reservation.StartTime);

        if (isConflict)
            return Conflict("Reservation time conflicts with an existing reservation for this room.");

        reservation.Id = DataStore.Reservations.Any() ? DataStore.Reservations.Max(r => r.Id) + 1 : 1;
        DataStore.Reservations.Add(reservation);

        return CreatedAtAction(nameof(GetReservation), new { id = reservation.Id }, reservation);
    }

    // PUT: /api/reservations/{id}
    [HttpPut("{id}")]
    public IActionResult UpdateReservation(int id, [FromBody] Reservation updatedReservation)
    {
        var existingReservation = DataStore.Reservations.FirstOrDefault(r => r.Id == id);
        if (existingReservation == null)
            return NotFound($"Reservation with ID {id} not found.");

        var room = DataStore.Rooms.FirstOrDefault(r => r.Id == updatedReservation.RoomId);
        if (room == null) return NotFound($"Room with ID {updatedReservation.RoomId} does not exist.");
        if (!room.IsActive) return BadRequest("Cannot move reservation to an inactive room.");

        var isConflict = DataStore.Reservations.Any(r =>
            r.Id != id &&
            r.RoomId == updatedReservation.RoomId &&
            r.Date.Date == updatedReservation.Date.Date &&
            r.StartTime < updatedReservation.EndTime &&
            r.EndTime > updatedReservation.StartTime);

        if (isConflict)
            return Conflict("Updated reservation time conflicts with another existing reservation.");

        existingReservation.RoomId = updatedReservation.RoomId;
        existingReservation.OrganizerName = updatedReservation.OrganizerName;
        existingReservation.Topic = updatedReservation.Topic;
        existingReservation.Date = updatedReservation.Date;
        existingReservation.StartTime = updatedReservation.StartTime;
        existingReservation.EndTime = updatedReservation.EndTime;
        existingReservation.Status = updatedReservation.Status;

        return Ok(existingReservation);
    }

    // DELETE: /api/reservations/{id}
    [HttpDelete("{id}")]
    public IActionResult DeleteReservation(int id)
    {
        var reservation = DataStore.Reservations.FirstOrDefault(r => r.Id == id);
        if (reservation == null)
            return NotFound($"Reservation with ID {id} not found.");

        DataStore.Reservations.Remove(reservation);
        return NoContent(); // 204
    }
}