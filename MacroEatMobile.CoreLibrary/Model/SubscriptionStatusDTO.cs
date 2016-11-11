using System;
using System.Collections.Generic;
using System.Text;

namespace MacroEatMobile.CoreLibrary.Model
{
    public class SubscriptionStatusDTO
    {
        public string SubscriptionProvider { get; set; }
        public DateTime? PeriodEnd { get; set; }
        public string SubscriptionType { get; set; }
        public DateTime? LastPaymentDate { get; set; }
        public bool IsRecurring { get; set; }
    }
}
