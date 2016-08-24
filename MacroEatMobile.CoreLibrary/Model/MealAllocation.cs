using System;
using System.Collections.Generic;

namespace MacroEatMobile.Core
{
	public class MealAllocation
	{
		public Nutrition Targets {
			get;
			set;
		}

		public string MealLabel {
			get;
			set;
		}

		public int DailyPercent {
			get;
			set;
		}
	}
}

