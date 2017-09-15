using Foundation;
using Google.Analytics;

namespace MakeMeMove.iOS.Utilities
{
    public class UnifiedAnalytics
    {

        private UnifiedAnalytics()
        {
            Gai.SharedInstance.GetTracker("UA-56251913-1");
        }

        private static UnifiedAnalytics _unifiedAnalytics;
        public static UnifiedAnalytics GetInstance()
        {
            return _unifiedAnalytics ?? (_unifiedAnalytics = new UnifiedAnalytics());
        }

        private static ITracker DefaultTracker
        {

            get
            {
#if DEBUG
                Gai.SharedInstance.DryRun = true;
#endif

                Gai.SharedInstance.DefaultTracker.SetAllowIdfaCollection(true);

                return Gai.SharedInstance.DefaultTracker;
            }
        }

        public void SendScreenHitOnDefaultTracker(string screenName)
        {
            Gai.SharedInstance.DefaultTracker.Set(GaiConstants.ScreenName, screenName);

            Gai.SharedInstance.DefaultTracker.Send(DictionaryBuilder.CreateScreenView().Build());
        }

        public void SetOnDefaultTracker(string key, string value)
        {
            DefaultTracker.Set(key, value);
        }
        private static void SendOnDefaultTracker(NSDictionary eventToSend)
        {
            DefaultTracker.Send(eventToSend);
        }


        public void CreateAndSendEventOnDefaultTracker(string category, string action, string label, long? value)
        {
            SendOnDefaultTracker(CreateEvent(category, action, label, value));
        }

        private static NSDictionary CreateEvent(string category, string action, string label, long? value)
        {
            return DictionaryBuilder.CreateEvent(category, action, label, value).Build();
        }
    }
}
