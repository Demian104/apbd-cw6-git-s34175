namespace TrainingCenterAPI.Data;

using TrainingCenterAPI.Models;

public static class DataStore
{
    public static List<Room> Rooms { get; set; } = new List<Room>();
    public static List<Reservation> Reservations { get; set; } = new List<Reservation>();

    static DataStore()
    {
        Rooms.AddRange(new[]
        {
            new Room { Id = 1, Name = "Sala A1", BuildingCode = "A", Floor = 1, Capacity = 30, HasProjector = true, IsActive = true },
            new Room { Id = 2, Name = "Lab 204", BuildingCode = "B", Floor = 2, Capacity = 24, HasProjector = true, IsActive = true },
            new Room { Id = 3, Name = "Sala C3", BuildingCode = "C", Floor = 3, Capacity = 15, HasProjector = false, IsActive = true },
            new Room { Id = 4, Name = "Magazyn", BuildingCode = "A", Floor = -1, Capacity = 5, HasProjector = false, IsActive = false }
        });

        Reservations.AddRange(new[]
        {
            new Reservation { Id = 1, RoomId = 1, OrganizerName = "Jan Nowak", Topic = "Szkolenie BHP", Date = new DateTime(2026, 5, 10), StartTime = new TimeSpan(9, 0, 0), EndTime = new TimeSpan(11, 0, 0), Status = "confirmed" },
            new Reservation { Id = 2, RoomId = 2, OrganizerName = "Anna Kowalska", Topic = "Warsztaty z HTTP", Date = new DateTime(2026, 5, 10), StartTime = new TimeSpan(10, 0, 0), EndTime = new TimeSpan(12, 30, 0), Status = "confirmed" }
        });
    }
}