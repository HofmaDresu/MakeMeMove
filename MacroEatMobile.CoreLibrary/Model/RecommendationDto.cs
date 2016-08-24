using System;
using System.Collections.Generic;
using System.Text;
using MacroEatMobile.Core;

namespace MacroEatMobile.CoreLibrary.Model
{
    public class RecommendationDto
    {
        public int TotalMealCombinationsCount { get; set; }
        public List<Recommendation> Meals { get; set; }
        public bool ResultsLimitedForFreeUser { get; set; }
        public int NumberOfAdditionalResultsForProUser { get; set; }
    }
}
