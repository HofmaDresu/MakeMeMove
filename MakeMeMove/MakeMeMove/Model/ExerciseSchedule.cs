using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Humanizer;
using SQLite;

namespace MakeMeMove.Model
{
    public enum SchedulePeriod
    {
        HalfHourly,
        Hourly,
        BiHourly
    }

    [Table("ExerciseSchedules")]
    public class ExerciseSchedule
    {
        public ExerciseSchedule()
        {
            Exercises = new List<ExerciseBlock>();
        }

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        
        public SchedulePeriod Period { get; set; }
        [Ignore]
        public string PeriodDisplayString => Period.Humanize();
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        [Ignore]
        public List<ExerciseBlock> Exercises { get; set; }


        public static ExerciseSchedule CreateDefaultSchedule()
        {
            return new ExerciseSchedule
            {
                Period = SchedulePeriod.HalfHourly,
                StartTime = new DateTime(1, 1, 1, 8, 0, 0),
                EndTime = new DateTime(1, 1, 1, 19, 0, 0),
                Exercises = new List<ExerciseBlock>
                {
                    new ExerciseBlock
                    {
                        Name = PreBuiltExersises.PushUps.Humanize(),
                        Type = PreBuiltExersises.PushUps,
                        Quantity = 10,
                        Enabled = true
                    },
                    new ExerciseBlock
                    {
                        Name = PreBuiltExersises.SitUps.Humanize(),
                        Type = PreBuiltExersises.SitUps,
                        Quantity = 10,
                        Enabled = true
                    },
                    new ExerciseBlock
                    {
                        Name = PreBuiltExersises.JumpingJacks.Humanize(),
                        Type = PreBuiltExersises.JumpingJacks,
                        Quantity = 10,
                        Enabled = true
                    },
                    new ExerciseBlock
                    {
                        Name = PreBuiltExersises.Squats.Humanize(),
                        Type = PreBuiltExersises.Squats,
                        Quantity = 10,
                        Enabled = true
                    },
                    new ExerciseBlock
                    {
                        Name = PreBuiltExersises.CalfRaises.Humanize(),
                        Type = PreBuiltExersises.CalfRaises,
                        Quantity = 10,
                        Enabled = true
                    },
                    new ExerciseBlock
                    {
                        Name = PreBuiltExersises.Lunges.Humanize(),
                        Type = PreBuiltExersises.Lunges,
                        Quantity = 5,
                        Enabled = true
                    }
                }
            };
        }

    }
}
