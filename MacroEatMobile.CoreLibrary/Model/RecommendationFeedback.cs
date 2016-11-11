using System;

namespace MacroEatMobile.Core
{
	public class RecommendationFeedback
	{
		public int Id { get; set; }

		public int PersonId { get; set; }

		public int RecommendationId { get; set; }

		public int SearchId { get; set; }

		public int EatableMealId { get; set; }

		public string RestaurantId {
			get;
			set;
		}

		public FeedbackType FeedbackType { get; set; }

		public int Value { get; set; }
	}

	public enum FeedbackType
	{
		Recommendation = 1,
		MealQuantity,
		EatableMeal,
		Restaurant
	}

	public enum RestaurantFeedbackVals{
		NotExist=1,
		Good,
		Bad
	}

	public enum EatableMealFeedbackVals {
		NotOnMenu = 1,
		WrongNutrition,
		Good,
		Bad
	}

}

