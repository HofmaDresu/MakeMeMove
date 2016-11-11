using System.Collections.Generic;
using System.Threading.Tasks;
using MacroEatMobile.Core.UtilityInterfaces;
using MacroEatMobile.CoreLibrary.Model;
using MacroEatMobile.iPhone.Utilities;
using System.Net.Http;

namespace MacroEatMobile.Core
{
	public class fudistRestaurantAdapter :BaseServerAdapter
	{
		public static async Task<RestaurantDTO> GetByRestaurantsByLocation(IAuthorization authMgr, IUnifiedAnalytics unifiedAnalytics, int? searchId, string search,
            string location, int page, MacrosVariance macrosVariance)
		{
			var request = CreateRestRequest(Configuration.FudistRestDirectory, "api/recommendation/RestaurantsByLocation", HttpMethod.Get);

			request.AddParameter("search", search ?? ""); 
			request.AddParameter("searchId", searchId ?? 0); 
			request.AddParameter("page", page==0 ? 1 : page); 
			request.AddParameter("location", location); 
			request.AddParameter("macrosVariance", JsonSerializer.Serialize(macrosVariance)); 

			request.AddHeader ("Authorization", await GetAndRenewAuthHeader (authMgr));

            return await ExecuteFudistRequest<RestaurantDTO>(request, unifiedAnalytics);
			                                         
		}


        public static async Task<List<RestaurantBrand>> GetByRestaurantsByNameAndNutrition(IAuthorization AuthMgr, IUnifiedAnalytics unifiedAnalytics, 
            string search, MacrosVariance macrosVariance)
		{
			var request = CreateRestRequest (Configuration.FudistRestDirectory, "api/recommendation/do/RestaurantsByNameAndNutrition", HttpMethod.Get);

			request.AddParameter ("search", search); 
			request.AddParameter ("macrosVariance", JsonSerializer.Serialize (macrosVariance));

            request.AddHeader("Authorization", await GetAndRenewAuthHeader(AuthMgr));
            return await ExecuteFudistRequest<List<RestaurantBrand>>(request, unifiedAnalytics);

		}

        public static async Task<List<RestaurantBrand>> GetByRestaurantsByName(IAuthorization AuthMgr, IUnifiedAnalytics unifiedAnalytics, string search)
		{
			var request = CreateRestRequest (Configuration.FudistRestDirectory, "api/recommendation/do/RestaurantsByName", HttpMethod.Get);

			request.AddParameter ("search", search); 

			request.AddHeader ("Authorization", await GetAndRenewAuthHeader (AuthMgr));

            return await ExecuteFudistRequest<List<RestaurantBrand>>(request, unifiedAnalytics);
        
		}
	}

}

