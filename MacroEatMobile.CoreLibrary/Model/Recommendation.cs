using System;
using System.Collections.Generic;

namespace MacroEatMobile.Core
{

	public class Recommendation
	{

		public int Id { get; set; }

		public List<RecommendedMeal> Meals { get; set; }

		public Nutrition TargetNutrients { get; set; }

		public MealType MealType { get; set; }

		public RecType Type { get; set; }

		public DateTime RecTime { get; set; }

		public bool Closed { get; set; }

		public enum RecType
		{
			Restaurant,
			General,
			Packaged,
			Recipe
		}
	}

	public enum MealType
	{
		Unknown=-3,
		Snack=-2,
		Dessert=-1,
		Cocktail=0,
		Breakfast=1,
		Lunch=2,
		Dinner=3    
	}
}

