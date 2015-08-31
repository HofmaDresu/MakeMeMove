using System;
using System.Collections.Generic;
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
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public List<ExerciseBlock> Exercises { get; set; }


        public static ExerciseSchedule CreateDefaultSchedule()
        {
            return new ExerciseSchedule
            {
                Period = SchedulePeriod.HalfHourly,
                StartTime = new DateTime(1, 1, 1, 8, 0, 0),
                EndTime = new DateTime(1, 1, 1, 22, 0, 0),
                Exercises = new List<ExerciseBlock>
                {
                    new ExerciseBlock
                    {
                        Name = PreBuiltExersises.PushUps.Humanize(LetterCasing.LowerCase),
                        Quantity = 10
                    },
                    new ExerciseBlock
                    {
                        Name = PreBuiltExersises.SitUps.Humanize(LetterCasing.LowerCase),
                        Quantity = 10
                    },
                    new ExerciseBlock
                    {
                        Name = PreBuiltExersises.JumpingJacks.Humanize(LetterCasing.LowerCase),
                        Quantity = 10
                    },
                    new ExerciseBlock
                    {
                        Name = PreBuiltExersises.Squats.Humanize(LetterCasing.LowerCase),
                        Quantity = 10
                    }
                }
            };
        }

    }
}
