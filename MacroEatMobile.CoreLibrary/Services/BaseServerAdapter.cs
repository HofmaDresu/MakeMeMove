using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MacroEatMobile.iPhone.Utilities;
using System.Net;
using System.Reflection;
using MacroEatMobile.Core.UtilityInterfaces;
using System.Net.Http;
using ModernHttpClient;

namespace MacroEatMobile.Core
{
    public enum DataFormat
    {
        Unknown,
        Json
    }

    public class FudistRestRequest
    {
        private readonly HttpClient _client;
        private readonly HttpRequestMessage _request;
        private readonly List<Tuple<string, string>> _headers;
        private readonly Dictionary<string, object> _parameters;
        private string _body;
        private DataFormat _contentType;

        public FudistRestRequest(string restServer, string restDirectory, string restPath, HttpMethod method,
            int timeoutSeconds = 30)
        {
            _client = new HttpClient(new NativeMessageHandler()) { Timeout = TimeSpan.FromSeconds(timeoutSeconds)};

            _request = new HttpRequestMessage(method, restServer + "/" + GetRestPath(restDirectory, restPath));
            _headers = new List<Tuple<string, string>>();
            _parameters = new Dictionary<string, object>();
            _body = null;
            _contentType = DataFormat.Unknown;
        }

        public HttpRequestMessage GetRequestMessage()
        {
            if (_body != null)
            {
                StringContent content;
                switch (_contentType)
                {
                    case DataFormat.Unknown:
                        content = new StringContent(_body, Encoding.UTF8);
                        break;
                    case DataFormat.Json:
                        content = new StringContent(_body, Encoding.UTF8, "application/json");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                _request.Content = content;
            }

            _request.Headers.Clear();

            foreach (var h in _headers)
            {
                _request.Headers.Add(h.Item1, h.Item2);
            }

            if (_parameters.Count > 0)
            {
                _request.RequestUri =
                    new Uri(_request.RequestUri + "?" +
                            string.Join("&",
                                _parameters.Select(
                                    p => WebUtility.UrlEncode(p.Key) + "=" + WebUtility.UrlEncode(p.Value.ToString()))));
            }


            return _request;
        }

        public FudistRestRequest AddParameter(string parameterName, object value)
        {
            _parameters.Add(parameterName, value.ToString());
            return this;
        }

        public FudistRestRequest AddHeader(string name, string value)
        {
            _headers.Add(new Tuple<string, string>(name, value));
            return this;
        }

        public FudistRestRequest AddBody(string body)
        {
            _body = body;
            return this;
        }

        public FudistRestRequest SetDataFormat(DataFormat format)
        {
            _contentType = format;
            return this;
        }

    public async Task<HttpResponseMessage> ExecuteRequest ()
		{
			return await _client.SendAsync (GetRequestMessage ());
		}
        
		public static string GetRestPath(string restDirectory, string restPath)
		{
			return restDirectory + (string.IsNullOrEmpty(restDirectory) || restDirectory.EndsWith("/") ? string.Empty : "/") + restPath;
		}
    }

    public class BaseServerAdapter
    {
        public static FudistRestRequest CreateRestRequest(string restDirectory, string restPath, HttpMethod method, int timeoutSeconds = 30)
        {
			return new FudistRestRequest(Configuration.FudistRestServer, restDirectory, restPath, method, timeoutSeconds);
        }

		public async static Task<HttpResponseMessage> ExecuteFudistRequest(FudistRestRequest request, IUnifiedAnalytics analytics)
        {
			HttpResponseMessage response;

            try
            {
				response = await request.ExecuteRequest();
                
				if (response.StatusCode == HttpStatusCode.RequestTimeout || response.StatusCode == HttpStatusCode.GatewayTimeout)
				{
					analytics.CreateAndSendEventOnDefaultTracker("NETWORK ERROR", "Time out encountered", request.GetRequestMessage().RequestUri.AbsolutePath, null);
                    throw new TimeoutException();
                }
            }
            catch (Exception e)
            {
				analytics.CreateAndSendEventOnDefaultTracker("NETWORK ERROR", "Error encountered", request.GetRequestMessage().RequestUri.AbsolutePath, null);
                throw new NetworkException(e);
            }
			if (request.GetRequestMessage().Method == HttpMethod.Get && ! response.IsSuccessStatusCode)
            {
				analytics.CreateAndSendEventOnDefaultTracker("APPLICATION ERROR", "Bad Status Code", request.GetRequestMessage().RequestUri.AbsolutePath + " returned a status code of " + response.StatusCode, null);
                throw new NetworkException(null);
            }

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                //always remember to test for handling crypto errors because users token doesn't decrypt
                throw new NetworkException(new UnauthorizedAccessException());
            }

            return response;
        }

        public static async Task<T> ExecuteFudistRequest<T>(FudistRestRequest request, IUnifiedAnalytics analytics, bool reCheckApiFileAndRetryOnError = true) where T : class, new()
        {
			HttpResponseMessage response;
			try
			{
				response = await request.ExecuteRequest();
                
				if (response.StatusCode == HttpStatusCode.RequestTimeout || response.StatusCode == HttpStatusCode.GatewayTimeout)
				{
					analytics.CreateAndSendEventOnDefaultTracker("NETWORK ERROR", "Time out encountered", request.GetRequestMessage().RequestUri.AbsolutePath, null);
					throw new TimeoutException();
				}
			}
			catch (Exception e)
			{
				analytics.CreateAndSendEventOnDefaultTracker("NETWORK ERROR", "Error encountered", request.GetRequestMessage().RequestUri.AbsolutePath, null);
				throw new NetworkException(e);
			}
			if (request.GetRequestMessage().Method == HttpMethod.Get && ! response.IsSuccessStatusCode)
			{
				analytics.CreateAndSendEventOnDefaultTracker("APPLICATION ERROR", "Bad Status Code", request.GetRequestMessage().RequestUri.AbsolutePath + " returned a status code of " + response.StatusCode, null);
				throw new NetworkException(null);
			}

			if (request.GetRequestMessage().Method == HttpMethod.Put && response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.NoContent)
			{
				analytics.CreateAndSendEventOnDefaultTracker("APPLICATION ERROR", "PUT request failed", request.GetRequestMessage().RequestUri.AbsolutePath, null);
                throw new Exception("Update didn't take");
            }

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                //always remember to test for handling crypto errors because users token doesn't decrypt
                throw new NetworkException(new UnauthorizedAccessException());
            }

			var responseData = JsonSerializer.Deserialize<T> (await response.Content.ReadAsStringAsync());

			var objectIsEnumerable = responseData != null && responseData.GetType().GetTypeInfo().ImplementedInterfaces.Select(t => t.GetTypeInfo()).Any(ti => ti.IsGenericType && ti.GetGenericTypeDefinition() == typeof(ICollection<>));

            if (objectIsEnumerable ? !ValidateListType(responseData) : !ValidateType(responseData))
            {
                if (reCheckApiFileAndRetryOnError)
                {
                    var oldRestDirectory = Configuration.FudistRestDirectory;
                    await fudistConfigAdapter.CheckApiLocator(analytics);
					request.GetRequestMessage().RequestUri = new Uri( request.GetRequestMessage().RequestUri.AbsolutePath.Replace(oldRestDirectory, Configuration.FudistRestDirectory));

					return await ExecuteFudistRequest<T>(request, analytics, false);
                }
                else
                {
					analytics.CreateAndSendEventOnDefaultTracker("APPLICATION ERROR", "Incorrect api version",  request.GetRequestMessage().RequestUri.AbsolutePath, null);
                    throw new NetworkException(new Exception("Object types don't match"));
                }
            }

            return responseData;  
        }

        private static bool ValidateListType<T>(T listObj)
        {
            if (listObj == null || !(listObj as IEnumerable<object>).Any())
            {
                // Consider null or empty valid
                return true;
            }

            return (listObj as IEnumerable<object>).Any(ValidateType);
        }

        private static bool ValidateType<T>(T obj) where T : class
        {
            // Null object is considered valid
            return obj == null ||
				obj.GetType().GetTypeInfo()
					.DeclaredProperties.Where(pi=>pi.GetMethod.IsPublic && !pi.GetMethod.IsStatic) 
                    .Any(p => p.GetValue(obj) != null);
        }

		protected static async Task<string> GetAndRenewAuthHeader(IAuthorization authz)
		{
			var authToken = authz.GetAuthToken ();

			if (authToken.ExpirationDate == null || authToken.ExpirationDate < (DateTime.Now + TimeSpan.FromMinutes (3))) {

			    var client = new HttpClient(new NativeMessageHandler())
                {
			        BaseAddress = new Uri(Configuration.FudistRestServer),
			        Timeout = TimeSpan.FromSeconds(10000)
			    };

			    var body = $"grant_type=refresh_token&refresh_token={WebUtility.UrlEncode(authToken.RefreshToken)}";
				var content = new StringContent (body, Encoding.UTF8, "application/x-www-form-urlencoded");

				var response = await client.PostAsync (Configuration.FudistRestDirectory + "Token", content);

				if (response.StatusCode == HttpStatusCode.RequestTimeout || response.StatusCode == HttpStatusCode.GatewayTimeout)
				{
					throw new TimeoutException();
				}
				if (response.StatusCode == HttpStatusCode.BadRequest) 
				{
					authz.ClearPerson ();

					var x = new NetworkException (new UnauthorizedAccessException ());
					throw x;
				}
				if (response.StatusCode != HttpStatusCode.OK)
					return null;

				var responseData = JsonSerializer.Deserialize<TokenResponse>( await response.Content.ReadAsStringAsync());

				authToken = new AuthToken {
					AccessToken = responseData.access_token, 
					ExpirationDate = DateTime.Now + TimeSpan.FromSeconds(responseData.expires_in),
					RefreshToken = responseData.refresh_token
				};

				authz.SetAuthToken (authToken);

			}

			return $"Bearer {authToken.AccessToken}";

		}

		protected class TokenResponse {
			public double expires_in {
				get;
				set;
			}
			public string access_token {
				get;
				set;
			}
			public string refresh_token {
				get;
				set;
			}
		}
    }
}
