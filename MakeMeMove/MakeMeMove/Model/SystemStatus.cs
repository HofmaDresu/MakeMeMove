using SQLite;

namespace MakeMeMove.Model
{
    [Table("SystemStatus")]
    public class SystemStatus
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public bool IosServiceIsRunning { get; set; }
        public bool IsFirstRun { get; set; }
    }
}
