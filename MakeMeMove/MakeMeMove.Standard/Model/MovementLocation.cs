using SQLite;

namespace MakeMeMove.Standard.Model
{
    [Table("MovementLocations")]
    public class MovementLocation
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Name { get; set; }
        public (double Latitude, double Longitude) GeoCoordinate { get; set; }
    }
}
