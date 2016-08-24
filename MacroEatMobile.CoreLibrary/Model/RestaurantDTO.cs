using System;
using System.Collections.Generic;
using System.Text;
using MacroEatMobile.Core;

namespace MacroEatMobile.CoreLibrary.Model
{
    public class RestaurantDTO
    {
        public List<Restaurant> Restaurants { get; set; }
        public bool ResultsLimitedForFreeUser { get; set; }
        public int NumberOfAdditionalResultsForProUser { get; set; }
    }
}
