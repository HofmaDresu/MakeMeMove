using System;
using System.Collections.Generic;

namespace MacroEatMobile.Core
{
	public class MealOfDay
	{

		public MealOfDay ()
		{
			Foods = new List<Meal> ();
		}

		public string Title { get; set; }

		public List<Meal> Foods { get; set; }
	}

}

