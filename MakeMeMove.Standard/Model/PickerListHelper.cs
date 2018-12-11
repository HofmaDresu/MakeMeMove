using System;
using System.Linq;
using System.Collections.Generic;
using Humanizer;

namespace MakeMeMove.Model
{
	public static class PickerListHelper
	{
		public static List<string> GetExerciseTypeStrings()
		{
			return (from PreBuiltExersises suit in Enum.GetValues(typeof(PreBuiltExersises)) select suit.Humanize()).ToList();
		}

		public static List<string> GetExercisePeriods()
		{
			return (from SchedulePeriod suit in Enum.GetValues(typeof(SchedulePeriod)) select suit.Humanize()).ToList();
        }

        public static List<string> GetScheduleTypes()
        {
            // Remove 'except' when we're ready for custom
            return (from ScheduleType suit in Enum.GetValues(typeof(ScheduleType)).Cast<ScheduleType>().Except(new List<ScheduleType> { ScheduleType.Custom}) select suit.Humanize()).ToList();
        }
    }
}

