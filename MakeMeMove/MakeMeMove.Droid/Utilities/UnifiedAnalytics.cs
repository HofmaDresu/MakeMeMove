using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MacroEatMobile.Core.UtilityInterfaces;

namespace MakeMeMove.Droid.Utilities
{
    public class UnifiedAnalytics : IUnifiedAnalytics
    {

        private UnifiedAnalytics(Context context)
        {
            //AndroidAnalytics.Instance(context);
        }

        private static UnifiedAnalytics _unifiedAnalytics;
        public static UnifiedAnalytics GetInstance(Activity context)
        {
            if (_unifiedAnalytics == null)
            {
                _unifiedAnalytics = new UnifiedAnalytics(context);
                /*_unifiedAnalytics.DefaultTracker.EnableAutoActivityTracking(true);
                _unifiedAnalytics.DefaultTracker.EnableExceptionReporting(true);
                _unifiedAnalytics.Analytics.EnableAutoActivityReports(context.Application);*/
            }
            return _unifiedAnalytics;
        }

       /* private Tracker DefaultTracker => AndroidAnalytics.Instance().Tracker;

        private GoogleAnalytics Analytics => AndroidAnalytics.Instance().Analytics;*/

        public void SetOnDefaultTracker(string key, string value)
        {
            //TODO
        }

        public void CreateAndSendEventOnDefaultTracker(string category, string action, string label, long? value)
        {
            //TODO
        }

        public void SendScreenHitOnDefaultTracker(string screenName)
        {
            //TODO
        }
    }
}