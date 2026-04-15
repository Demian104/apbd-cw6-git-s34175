using Microsoft.AspNetCore.Mvc;
using TrainingCenterAPI.Data;
using TrainingCenterAPI.Models;

namespace TrainingCenterAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RoomsController : ControllerBase
{
    // GET: /api/rooms?minCapacity=20&hasProjector=true&activeOnly=true
    [HttpGet]
    public ActionResult<IEnumerable<Room>> GetRooms(
        [FromQuery] int? minCapacity, 
        [FromQuery] bool? hasProjector, 
        [FromQuery] bool? activeOnly)
    {
        var rooms = DataStore.Rooms.AsQueryable();

        if (minCapacity.HasValue)
            rooms = rooms.Where(r => r.Capacity >= minCapacity.Value);
        
        if (hasProjector.HasValue)
            rooms = rooms.Where(r => r.HasProjector == hasProjector.Value);
        
        if (activeOnly.HasValue && activeOnly.Value)
            rooms = rooms.Where(r => r.IsActive);

        return Ok(rooms.ToList());
    }

    // GET: /api/rooms/{id}
    [HttpGet("{id}")]
    public ActionResult<Room> GetRoom(int id)
    {
        var room = DataStore.Rooms.FirstOrDefault(r => r.Id == id);
        if (room == null)
            return NotFound($"Room with ID {id} not found.");

        return Ok(room);
    }

    // GET: /api/rooms/building/{buildingCode}
    [HttpGet("building/{buildingCode}")]
    public ActionResult<IEnumerable<Room>> GetRoomsByBuilding(string buildingCode)
    {
        var rooms = DataStore.Rooms
            .Where(r => r.BuildingCode.Equals(buildingCode, StringComparison.OrdinalIgnoreCase))
            .ToList();

        return Ok(rooms);
    }

    // POST: /api/rooms
    [HttpPost]
    public ActionResult<Room> CreateRoom([FromBody] Room room)
    {
        room.Id = DataStore.Rooms.Any() ? DataStore.Rooms.Max(r => r.Id) + 1 : 1;
        DataStore.Rooms.Add(room);

        return CreatedAtAction(nameof(GetRoom), new { id = room.Id }, room);
    }

    // PUT: /api/rooms/{id}
    [HttpPut("{id}")]
    public IActionResult UpdateRoom(int id, [FromBody] Room updatedRoom)
    {
        var existingRoom = DataStore.Rooms.FirstOrDefault(r => r.Id == id);
        if (existingRoom == null)
            return NotFound($"Room with ID {id} not found.");

        existingRoom.Name = updatedRoom.Name;
        existingRoom.BuildingCode = updatedRoom.BuildingCode;
        existingRoom.Floor = updatedRoom.Floor;
        existingRoom.Capacity = updatedRoom.Capacity;
        existingRoom.HasProjector = updatedRoom.HasProjector;
        existingRoom.IsActive = updatedRoom.IsActive;

        return Ok(existingRoom);
    }

    // DELETE: /api/rooms/{id}
    [HttpDelete("{id}")]
    public IActionResult DeleteRoom(int id)
    {
        var room = DataStore.Rooms.FirstOrDefault(r => r.Id == id);
        if (room == null)
            return NotFound($"Room with ID {id} not found.");

        var hasFutureReservations = DataStore.Reservations
            .Any(r => r.RoomId == id && r.Date >= DateTime.Today);

        if (hasFutureReservations)
            return Conflict($"Cannot delete room {id} because it has future reservations.");

        DataStore.Rooms.Remove(room);
        return NoContent(); // 204
    }
}