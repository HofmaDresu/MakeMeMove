using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MacroEatMobile.Core;

namespace MacroEatMobile.CoreLibrary.Model
{
    public class RegistrationSimpleDayAllocation
    {
        public List<RegistrationSimpleMealAllocation> SimpleMealAllocations { get; set; }
        public TargetMacros TotalMacros { get; set; }

        public TargetMacros GetMacrosForMeal(int mealNumber)
        {
            var thisMealsPercentOfWeight = GetDailyPercentForMeal(mealNumber);

            return new TargetMacros
            {
                Calories = (int)Math.Floor(thisMealsPercentOfWeight * TotalMacros.Calories),
                Fat = (int)Math.Floor(thisMealsPercentOfWeight * TotalMacros.Fat),
                Protein = (int)Math.Floor(thisMealsPercentOfWeight * TotalMacros.Protein),
                Carbohydrates = (int)Math.Floor(thisMealsPercentOfWeight * TotalMacros.Carbohydrates)
            };
        }

        public decimal GetDailyPercentForMeal(int mealNumber)
        {
            var totalWeight = SimpleMealAllocations.Sum(s => (int) s.MealSize);
            var thisMealsPercentOfWeight = (decimal) SimpleMealAllocations[mealNumber].MealSize/totalWeight;
            return thisMealsPercentOfWeight;
        }
    }

    public class RegistrationSimpleMealAllocation
    {
        public string MealName { get; set; }
        public SettingsMealSize MealSize { get; set; }
    }

    public enum SettingsMealSize
    {
        VerySmall = 5,
        Small = 15,
        Medium = 30,
        Large = 50, 
        VeryLarge = 70
    }
}
