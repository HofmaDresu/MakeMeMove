using System;
using System.Collections.Generic;
using System.Text;
using MacroEatMobile.Core;

namespace MacroEatMobile.CoreLibrary.Model
{
    public class EatableMealDTO
    {
        public List<Meal> EatableMeals { get; set; }
        public bool ResultsLimitedForFreeUser { get; set; }
        public int NumberOfAdditionalResultsForProUser { get; set; }
    }
}
