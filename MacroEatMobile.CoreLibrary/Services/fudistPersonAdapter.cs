using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using MacroEatMobile.Core.UtilityInterfaces;
using MacroEatMobile.iPhone.Utilities;
using ModernHttpClient;

namespace MacroEatMobile.Core
{
	public class FudistPersonAdapter : BaseServerAdapter
	{
        public static async Task<Person> GetAuthenticatedPerson(IAuthorization authZ, IUnifiedAnalytics unifiedAnalytics)
        {
            var request = CreateRestRequest(Configuration.FudistRestDirectory, "api/person/do/GetAuthenticatedPerson", HttpMethod.Get, 10);

            request.AddHeader ("Authorization", await GetAndRenewAuthHeader (authZ));
            
			var person = await ExecuteFudistRequest<Person>(request, unifiedAnalytics);
			if (person.Id == 0)
				return null;

            unifiedAnalytics.SetOnDefaultTracker("&uid", person.Username);
            unifiedAnalytics.CreateAndSendEventOnDefaultTracker("UX", "User Token Auth", null, null);

			return person;
		}

        //This is async in case we want to do more in the future, so we don't need to change signatures as that can be weird with tasks
	    public static async Task<Person> SignInAsGuest(IUnifiedAnalytics unifiedAnalytics, IAuthorization authMgr)
        {
            return await AuthPerson("guest@fudi.st", "iD8d^fh23(jdfghy", unifiedAnalytics, authMgr);
	    }

		public static async Task<Person> AuthPerson(string userName, string password, IUnifiedAnalytics unifiedAnalytics, IAuthorization authMgr)
        {
            if (userName == null) throw new ArgumentNullException(nameof(userName));
		    var showGuestLoginEvent = userName == "guest@fudi.st";

            var client = new HttpClient(new NativeMessageHandler())
            {
		        BaseAddress = new Uri(Configuration.FudistRestServer),
		        Timeout = TimeSpan.FromSeconds(10)
		    };

		    var body = $"grant_type=password&username={WebUtility.UrlEncode(userName)}&password={WebUtility.UrlEncode(password)}";
			var content = new StringContent (body, Encoding.UTF8, "application/x-www-form-urlencoded");

			var response = await client.PostAsync (Configuration.FudistRestDirectory + "Token", content);

			//TODO: TEST STATUS CODE FOR TIMEOUT IS ACTUALLY WHAT IS USED
			if (response.StatusCode == HttpStatusCode.RequestTimeout || response.StatusCode == HttpStatusCode.GatewayTimeout)
            {
                unifiedAnalytics.CreateAndSendEventOnDefaultTracker("NETWORK ERROR", "Time out encountered", "Token", null);
                throw new TimeoutException();
            }
			if ( ! response.IsSuccessStatusCode)
		    {
		        return null;
		    }

			var parsedResponse = JsonSerializer.Deserialize<TokenResponse> (await response.Content.ReadAsStringAsync());

		    var authToken = new AuthToken {
				AccessToken = parsedResponse.access_token,
				ExpirationDate = DateTime.Now + TimeSpan.FromSeconds(parsedResponse.expires_in),
				RefreshToken = parsedResponse.refresh_token
			};

			authMgr.SetAuthToken (authToken);



			var person = await GetAuthenticatedPerson (authMgr, unifiedAnalytics);

            unifiedAnalytics.SetOnDefaultTracker("&uid", userName);

		    if (showGuestLoginEvent)
            {
                unifiedAnalytics.CreateAndSendEventOnDefaultTracker("UX", "Guest Sign In", null, null);
            }
		    else
            {
                unifiedAnalytics.CreateAndSendEventOnDefaultTracker("UX", "User Sign In", null, null);
            }


			return person;
		}

        public static async Task UpdatePerson(Person person, IAuthorization authMgr, IUnifiedAnalytics unifiedAnalytics)
        {
            if (person.IsGuestUser) return;

			var request = CreateRestRequest(Configuration.FudistRestDirectory, $"api/person/{person.Id}", HttpMethod.Put);
            
            request.AddHeader ("Authorization", await GetAndRenewAuthHeader (authMgr));
            
            request.SetDataFormat(DataFormat.Json);
            request.AddBody (JsonSerializer.Serialize( person));
            
            await ExecuteFudistRequest<Person>(request, unifiedAnalytics);
        }

		public static async Task<Person> AuthPersonWithToken(string accessToken, string provider, IUnifiedAnalytics unifiedAnalytics, IAuthorization authMgr)
	    {
			
			var httpClientHandler = new HttpClientHandler {	CookieContainer = new CookieContainer(), AllowAutoRedirect = false };

		    var client = new HttpClient(httpClientHandler)
            {
		        BaseAddress = new Uri(Configuration.FudistRestServer),
		        Timeout = TimeSpan.FromSeconds(10)
		    };

			var content = new StringContent(string.Empty);
			content.Headers.Add(provider + "Token", accessToken);

			var response = await client.PostAsync (Configuration.FudistRestDirectory + provider + "Token", content);
           	//IRestResponse response = client.Execute(request1);

			var locHeader = response.Headers.First (hd => hd.Key == "Location").Value.First();
			response = await client.GetAsync(locHeader );

			var tokenUrl = new Uri(response.Headers.First(hd => hd.Key == "Location").Value.First());
			//var token = tokenUrl.Fragment.Split ('=','&') [1];
			//var retVals = tokenUrl.Fragment.Substring(1);

			var splitToken = tokenUrl.Fragment.Substring(1).Split ('=', '&');
			var retVals = splitToken.SelectMany ((str, ind) => {
				if (ind % 2 == 0)
                { 
					return new [] {Tuple.Create( str, splitToken[ind+1])};
                }
                
				return new Tuple<string,string> []{};
			}).ToDictionary(t=>t.Item1, t=>t.Item2);

			var authToken = new AuthToken {
				AccessToken = retVals ["access_token"],
				ExpirationDate = DateTime.Now + TimeSpan.FromSeconds (double.Parse (retVals["expires_in"])),
				RefreshToken = retVals.ContainsKey("refresh_token") ? retVals["refresh_token"] : ""
			};

			authMgr.SetAuthToken (authToken);

			var person = await GetAuthenticatedPerson(authMgr, unifiedAnalytics);

			unifiedAnalytics.SetOnDefaultTracker("&uid", person.Username);
			unifiedAnalytics.CreateAndSendEventOnDefaultTracker("UX", "User Sign In", null, null);

            return person;
	    }
	}
}