using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using MacroEatMobile.Core.UtilityInterfaces;
using MacroEatMobile.iPhone.Utilities;

namespace MacroEatMobile.Core
{
	public class fudistMfpAdapter : BaseServerAdapter
	{
        public static async Task<List<MealOfDay>> GetDiary(string mfpUsername, string mfpShareKey, IAuthorization authMgr, IUnifiedAnalytics analytics)
        {
            
                var request = CreateRestRequest(Configuration.FudistRestDirectory, "api/mfp/do/GetDiary", HttpMethod.Get, 10);
                request.AddParameter("mfpUsername", mfpUsername);
                request.AddParameter("shareKey", mfpShareKey);

                request.AddHeader("Authorization", await GetAndRenewAuthHeader(authMgr));

                var mfpMealDiary = await ExecuteFudistRequest<List<MealOfDay>>(request, analytics);

                analytics.CreateAndSendEventOnDefaultTracker("UX", "MFP GetDiary", null, null);

                return mfpMealDiary;
            
		}

        public static async Task<string> CanGetDiary(string mfpUsername, string mfpShareKey, IAuthorization AuthMgr, IUnifiedAnalytics analytics)
        {
            var request = CreateRestRequest(Configuration.FudistRestDirectory, "api/mfp/do/CanGetDiary", HttpMethod.Get, 10);
            request.AddParameter("mfpUsername", mfpUsername);
            request.AddParameter("shareKey", mfpShareKey); 

            request.AddHeader("Authorization", await GetAndRenewAuthHeader(AuthMgr));

			var response = await ExecuteFudistRequest (request, analytics);
				
			var canGet = (await response.Content.ReadAsStringAsync()).Trim("\"".ToCharArray());

			return canGet;
		}

	}
}

