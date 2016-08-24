using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using MacroEatMobile.Core.UtilityInterfaces;
using MacroEatMobile.CoreLibrary.Model;
using MacroEatMobile.iPhone.Utilities;

namespace MacroEatMobile.Core
{
	public class FudistSubscriptionAdapter : BaseServerAdapter
	{
        public static async Task<bool> MarkSubscribedThroughGoogle(IAuthorization authz, IUnifiedAnalytics unifiedAnalytics, int personId, string subscriptionId, string subscriptionPlan)
	    {
	        return await Task.Run(async () =>
	        {
	            var request = CreateRestRequest(Configuration.FudistRestDirectory,
	                "api/subscription/MarkSubscribedThroughGoogle", HttpMethod.Post);
	            request.AddHeader("Authorization", await GetAndRenewAuthHeader(authz));
                request.SetDataFormat(DataFormat.Json);
                request.AddBody(JsonSerializer.Serialize(new SubscribedThroughGoogleDTO
                {
                    PersonId = personId,
                    SubscriptionId= subscriptionId,
                    SubscriptionPlan = subscriptionPlan
                }));

	            var response = await ExecuteFudistRequest(request, unifiedAnalytics);

                return response.StatusCode == HttpStatusCode.OK;
	        });
	    }
        public static async Task<bool> MarkSubscribedThroughApple(IAuthorization authz, IUnifiedAnalytics unifiedAnalytics, int personId, string subscriptionPlan, string base64EncodedReceipt)
        {
            return await Task.Run(async() =>
            {
                var request = CreateRestRequest(Configuration.FudistRestDirectory,
                    "api/subscription/MarkSubscribedThroughApple", HttpMethod.Post);
                request.AddHeader("Authorization", await GetAndRenewAuthHeader(authz));

                request.SetDataFormat(DataFormat.Json);
                request.AddBody(JsonSerializer.Serialize(new SubscribedThroughAppleDTO
                {
                    PersonId = personId,
                    Base64EncodedReceipt = base64EncodedReceipt,
                    SubscriptionPlan = subscriptionPlan
                }));

					var response = await ExecuteFudistRequest(request, unifiedAnalytics);

                return response.StatusCode == HttpStatusCode.OK;
            });
        }

	    public static async Task ReportInAppBillingError(IUnifiedAnalytics unifiedAnalytics, string billingService, int personId, string product, string errorType, 
            string errorCode, string message, string base64EncodedReceipt = "")
	    {
        
			var request = CreateRestRequest(Configuration.FudistRestDirectory, "api/subscription/RecordInAppSubscriptionError", HttpMethod.Post);
            request.SetDataFormat(DataFormat.Json);
            request.AddBody(JsonSerializer.Serialize(new InAppBillingError
            {
                Base64EncodedReceipt = base64EncodedReceipt,
                PersonId = personId,
                BillingService = billingService,
                ErrorCode = errorCode,
                ErrorType = errorType,
                Message = message,
                Product = product
            }));
            

            var response = await ExecuteFudistRequest(request, unifiedAnalytics);

	    }

        public static async Task<SubscriptionStatusDTO> GetSubscriptionStatus(IAuthorization authz, IUnifiedAnalytics unifiedAnalytics, int personId)
        {
            var request = CreateRestRequest(Configuration.FudistRestDirectory,
                $"api/subscription/{personId}/GetUserSubscriptionStatus", HttpMethod.Get);
            request.AddHeader("Authorization", await GetAndRenewAuthHeader(authz));
            var response = await ExecuteFudistRequest<SubscriptionStatusDTO>(request, unifiedAnalytics);
            return response;
        }
	
        public static async Task<ActiveSubscriptionTypesResponseDTO> GetActiveSubscriptionIds(IUnifiedAnalytics unifiedAnalytics, string storeType)
        {      
            var request = CreateRestRequest(Configuration.FudistRestDirectory,
                "api/subscription/GetActiveSubscriptionTypes", HttpMethod.Get);
            request.AddParameter("storeType", storeType);

            var response = await ExecuteFudistRequest<ActiveSubscriptionTypesResponseDTO>(request, unifiedAnalytics);
            return response;
            
        }
	}
}