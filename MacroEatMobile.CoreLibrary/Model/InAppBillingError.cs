using System;
using System.Collections.Generic;
using System.Text;

namespace MacroEatMobile.CoreLibrary.Model
{
    public class InAppBillingError
    {
        public long Id { get; set; }
        public string BillingService { get; set; }
        public int PersonId { get; set; }
        public string Product { get; set; }
        public string ErrorType { get; set; }
        public string ErrorCode { get; set; }
        public string Message { get; set; }
        public DateTime ErrorDate { get; set; }
        public bool IsDealtWith { get; set; }
        public string Base64EncodedReceipt { get; set; }
    }
}
