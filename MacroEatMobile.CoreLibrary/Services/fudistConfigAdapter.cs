using System;
using System.Collections.Generic;
using System.Linq;
using MacroEatMobile.Core.UtilityInterfaces;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net;
using ModernHttpClient;

namespace MacroEatMobile.Core
{
	public static class fudistConfigAdapter
	{
		public static async Task Configure(string currentVersion, string operatingSystem, IUnifiedAnalytics analytics)
	    {
            Configuration.OperatingSystem = operatingSystem;
            Configuration.Version = currentVersion;
            await CheckApiLocator(analytics);
	    }

		public async static Task CheckApiLocator(IUnifiedAnalytics analytics)
		{
		    var client = new HttpClient(new NativeMessageHandler())
            {
		        BaseAddress = new Uri(Configuration.FudistRestServer + "/" + Configuration.FudistRestDirectory),
		        Timeout = TimeSpan.FromSeconds(10000)
		    };

		    var response2 = await client.GetAsync ("apiLocator.json");

            var installedVersion = Version.Parse(Configuration.Version);

			string configString;

			if (response2.StatusCode == HttpStatusCode.RequestTimeout || response2.StatusCode == HttpStatusCode.GatewayTimeout)
			{
                analytics.CreateAndSendEventOnDefaultTracker("FATAL NETWORK ERROR", "Time out encountered calling apiLocator.", null, null);
                throw new TimeoutException();
            }

            if (response2.StatusCode != HttpStatusCode.OK)
            {
                analytics.CreateAndSendEventOnDefaultTracker("FATAL NETWORK ERROR", "Error reading apiLocator.", null, null);
                throw new NetworkException(null);
			} else {
				configString = await response2.Content.ReadAsStringAsync();
			}
		    
			var configSet = JsonSerializer.Deserialize<AdaptedConfigSet> (configString);

			var config = configSet.Configs.First (cfg => {
				var cfgVersion = Version.Parse(cfg.Version);
                return cfgVersion.CompareTo(installedVersion) <= 0 && cfg.OperatingSystem == Configuration.OperatingSystem;
			});

			Configuration.FudistRestServer = config.FudistRestServer;
			Configuration.FudistRestDirectory = config.FudistRestDirectory;
		}
	}

	class AdaptedConfigSet
	{
		public List<AdaptedConfig> Configs {
			get;
			set;
		}
	}

	class AdaptedConfig {
		public string Version {
			get;
			set;
		}
        public string OperatingSystem
        {
            get;
            set;
        }
		public string FudistRestServer {
			get;
			set;
		}
		public string FudistRestDirectory {
			get;
			set;
		}
		public string Message {
			get;
			set;
		}
		public bool ForceClose {
			get;
			set;
		}
	}
}