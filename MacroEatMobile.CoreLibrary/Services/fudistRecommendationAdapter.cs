using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MacroEatMobile.Core.UtilityInterfaces;
using MacroEatMobile.CoreLibrary.Model;
using MacroEatMobile.iPhone.Utilities;
using System.Net.Http;

namespace MacroEatMobile.Core
{
	public class FudistRecommendationAdapter : BaseServerAdapter
	{
		public FudistRecommendationAdapter ()
		{
		}

        public static async Task<List<Recommendation>> GetPreSetRecommendationsForBrand(IAuthorization authMgr, IUnifiedAnalytics unifiedAnalytics, int restaurantBrandId, MacrosVariance macrosVariance
            , List<int> foodsToInclude = null)
		{
			
				var request = CreateRestRequest(Configuration.FudistRestDirectory, "api/recommendation/do/GetPreSetRecommendationsForBrand", HttpMethod.Get);

				request.AddParameter("restaurantBrandId", restaurantBrandId); 
				request.AddParameter("macrosVariance", JsonSerializer.Serialize(macrosVariance)); 
				request.AddParameter("mealsSoFar", string.Join(",",foodsToInclude ?? new List<int>()));

				request.AddHeader ("Authorization", await GetAndRenewAuthHeader (authMgr));

                return await ExecuteFudistRequest<List<Recommendation>>(request, unifiedAnalytics);
			                                      
		}

        public static async Task<RecommendationDto> GetRecommendationsForUser(IAuthorization authMgr, IUnifiedAnalytics unifiedAnalytics, int restaurantBrandId, 
            MacrosVariance macrosVariance, int personId)
        {
            
            var request = CreateRestRequest(Configuration.FudistRestDirectory, "api/recommendation/GetRecommendationsForUser", HttpMethod.Get);

            request.AddParameter("restaurantBrandId", restaurantBrandId);
            request.AddParameter("macrosVariance", JsonSerializer.Serialize(macrosVariance));
            request.AddParameter("personId", personId);
            request.AddParameter("maxResults", 10);     
			request.AddHeader("Authorization", await GetAndRenewAuthHeader (authMgr));

            return await ExecuteFudistRequest<RecommendationDto>(request, unifiedAnalytics);
        
        }


        public static async Task<EatableMealDTO> GetMealMatchesForBrand(IAuthorization authMgr, IUnifiedAnalytics unifiedAnalytics, int restaurantBrandId, MacrosVariance macrosVariance, 
            List<int> mealsSoFar)
		{
			
			var request = CreateRestRequest(Configuration.FudistRestDirectory, "api/recommendation/GetMealMatchesForBrand", HttpMethod.Get);

			request.AddParameter("restaurantBrandId", restaurantBrandId); 
			request.AddParameter("macrosVariance", JsonSerializer.Serialize(macrosVariance)); 
			request.AddParameter("mealsSoFar", string.Join(",",mealsSoFar));

			request.AddHeader ("Authorization", await GetAndRenewAuthHeader (authMgr));

            return await ExecuteFudistRequest<EatableMealDTO>(request, unifiedAnalytics);   
			                                      
		}


        public static async Task SendFeedback(IAuthorization authMgr, IUnifiedAnalytics unifiedAnalytics, RecommendationFeedback feedback)
		{
			
			var request = CreateRestRequest(Configuration.FudistRestDirectory, $"api/recommendation/RecommendationFeedback?recommendationFeedback={JsonSerializer.Serialize(feedback)}", HttpMethod.Post);

			//request.AddParameter("recommendationFeedback", JsonSerializer.Serialize(feedback), ParameterType.UrlSegment); 
			request.AddHeader ("Authorization", await GetAndRenewAuthHeader (authMgr));

            await ExecuteFudistRequest(request, unifiedAnalytics);

			return;
		}


		public static Nutrition RecommendationNutritionAggregator(Recommendation recommendation)
		{

			var aggregate = recommendation.Meals
				.Select (m => new { m.Quantity, m.EatableMeal.Nutrition})
				.Aggregate (
					new {
						Quantity = 0m,
						Nutrition = new Nutrition() {
							Calories = 0,
							Fat = 0,
							Protein = 0,
							Carbohydrates = 0,
							SaturatedFat  = 0,		
							TransFat 	 = 0,	
							Sugar 		 = 0,
							Fiber 		 = 0,
							Cholesterol  = 0,		
							Sodium 		 = 0,
							VitaminA 	 = 0,	
							VitaminC 	 = 0,	
							Iron 		 = 0,
							Calcium 	 = 0
						}
					},
					(m1, m2) => new {
						Quantity = m1.Quantity+m1.Quantity,  //post aggregation the quantity is meaningless
						Nutrition = new Nutrition () { 
							Calories = m1.Nutrition.Calories + m2.Quantity * m2.Nutrition.Calories,
							Fat = m1.Nutrition.Fat + m2.Quantity * m2.Nutrition.Fat,
							Protein = m1.Nutrition.Protein + m2.Quantity * m2.Nutrition.Protein,
							Carbohydrates = m1.Nutrition.Carbohydrates + m2.Quantity * m2.Nutrition.Carbohydrates,
							SaturatedFat = m1.Nutrition.SaturatedFat + m2.Quantity * m2.Nutrition.SaturatedFat,
							TransFat = m1.Nutrition.TransFat + m2.Quantity * m2.Nutrition.TransFat,
							Sugar = m1.Nutrition.Sugar + m2.Quantity * m2.Nutrition.Sugar,
							Fiber = m1.Nutrition.Fiber + m2.Quantity * m2.Nutrition.Fiber,
							Cholesterol = m1.Nutrition.Cholesterol + m2.Quantity * m2.Nutrition.Cholesterol,
							Sodium = m1.Nutrition.Sodium + m2.Quantity * m2.Nutrition.Sodium,
							VitaminA = m1.Nutrition.VitaminA + m2.Quantity * m2.Nutrition.VitaminA,
							VitaminC = m1.Nutrition.VitaminC + m2.Quantity * m2.Nutrition.VitaminC,
							Iron = m1.Nutrition.Iron + m2.Quantity * m2.Nutrition.Iron,
							Calcium = m1.Nutrition.Calcium + m2.Quantity * m2.Nutrition.Calcium
						}
					}
				);

			return aggregate.Nutrition;
		}

	}
}

