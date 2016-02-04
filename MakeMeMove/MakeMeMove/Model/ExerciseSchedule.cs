using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Humanizer;

namespace MakeMeMove.Model
{
    public enum SchedulePeriod
    {
        HalfHourly,
        Hourly,
        BiHourly
    }

    public class ExerciseSchedule
    {
        public ExerciseSchedule()
        {
            Exercises = new List<ExerciseBlock>();
        }

        public SchedulePeriod Period { get; set; }
        public string PeriodDisplayString => Period.Humanize();
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
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
                        Id = Guid.NewGuid(),
                        Name = PreBuiltExersises.PushUps.Humanize(),
                        Type = PreBuiltExersises.PushUps,
                        Quantity = 10,
                        Enabled = true
                    },
                    new ExerciseBlock
                    {
                        Id = Guid.NewGuid(),
                        Name = PreBuiltExersises.SitUps.Humanize(),
                        Type = PreBuiltExersises.SitUps,
                        Quantity = 10,
                        Enabled = true
                    },
                    new ExerciseBlock
                    {
                        Id = Guid.NewGuid(),
                        Name = PreBuiltExersises.JumpingJacks.Humanize(),
                        Type = PreBuiltExersises.JumpingJacks,
                        Quantity = 10,
                        Enabled = true
                    },
                    new ExerciseBlock
                    {
                        Id = Guid.NewGuid(),
                        Name = PreBuiltExersises.Squats.Humanize(),
                        Type = PreBuiltExersises.Squats,
                        Quantity = 10,
                        Enabled = true
                    },
                    new ExerciseBlock
                    {
                        Id = Guid.NewGuid(),
                        Name = PreBuiltExersises.CalfRaises.Humanize(),
                        Type = PreBuiltExersises.CalfRaises,
                        Quantity = 10,
                        Enabled = true
                    },
                    new ExerciseBlock
                    {
                        Id = Guid.NewGuid(),
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
