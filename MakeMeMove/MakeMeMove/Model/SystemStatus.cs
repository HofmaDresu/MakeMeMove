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
        public bool? AskForRating_DB_ONLY { get; set; }
        public int RatingCheckTimesOpened { get; set; }
        [Ignore]
        public bool AskForRating => AskForRating_DB_ONLY.GetValueOrDefault(true) && RatingCheckTimesOpened >= 10;
    }
}
