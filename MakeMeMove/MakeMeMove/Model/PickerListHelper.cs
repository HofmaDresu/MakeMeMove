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
	}
}

