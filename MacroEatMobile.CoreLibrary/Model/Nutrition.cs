using System;

namespace MacroEatMobile.Core
{
	public class Nutrition
	{
		public Nutrition ()
		{
		}

		public int Id { get; set; }


		public decimal? Calories { get; set; }
		public decimal? Protein { get; set; }
		public decimal? Fat { get; set; }
		public decimal? Carbohydrates { get; set; }
		public decimal? ServingsPerContainer { get; set; }
		public decimal? ServingSizeQty { get; set; }
		public string   ServingSizeUnit { get; set; }
		public decimal? ServingMetric { get; set; }
		public string   ServingMetricUnit { get; set; }
		public string   ServingSizeDesc { get; set; }
		public decimal? Sodium { get; set; }
		public decimal? SaturatedFat { get; set; }
		public decimal? PolyUnsaturatedFat { get; set; }
		public decimal? MonoUnsaturatedFat { get; set; }
		public decimal? TransFat { get; set; }
		public decimal? Cholesterol { get; set; }
		public decimal? Potassium { get; set; }
		public decimal? Fiber { get; set; }
		public decimal? Sugar { get; set; }
		public decimal? VitaminA { get; set; }
		public decimal? VitaminC { get; set; }
		public decimal? Calcium { get; set; }
		public decimal? Iron { get; set; }
	}
}

