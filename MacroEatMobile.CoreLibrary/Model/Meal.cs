using System;

namespace MacroEatMobile.Core
{
	public class RecommendedMeal
	{
		public RecommendedMeal()
		{
			Quantity = 1;
		}

		public int Id { get; set; }

		public Meal EatableMeal { get; set; }

		public decimal Quantity { get; set; }
	}

	public class Meal
	{
		public Meal ()
		{
		}

		//public IList<ScoringAttribute> ScoringAttributes { get; set; }
		public int Id { get; set; }
		public string Name { get; set; }
		public string Brand { get; set; }
		public string Description { get; set; }
		public Nutrition Nutrition { get; set; }
	    public string OriginalSource { get; set; }
        public bool LowConfidence { get { return OriginalSource == "f"; } }
	}
}

