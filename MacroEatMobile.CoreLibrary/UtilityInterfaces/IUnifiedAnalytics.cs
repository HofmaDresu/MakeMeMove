using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MacroEatMobile.Core.UtilityInterfaces
{
    public interface IUnifiedAnalytics
    {
        void SetOnDefaultTracker(string key, string value);
        void CreateAndSendEventOnDefaultTracker(string category, string action, string label, long? value);
        void SendScreenHitOnDefaultTracker(string screenName);

    }
}
