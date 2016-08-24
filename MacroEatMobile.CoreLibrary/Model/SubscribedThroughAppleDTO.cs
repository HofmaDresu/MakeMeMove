using System;
using System.Collections.Generic;
using System.Text;

namespace MacroEatMobile.CoreLibrary.Model
{
    public class SubscribedThroughAppleDTO
    {
        public int PersonId { get; set; }
        public string Base64EncodedReceipt { get; set; }
        public string SubscriptionPlan { get; set; }
    }
}
