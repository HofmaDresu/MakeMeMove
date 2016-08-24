using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MacroEatMobile.Core
{
    public enum DayAllocationType
    {
        None = 0,
        Normal = 1,
        Training = 2,
        Resting = 3
    }

    public class DayAllocation
    {
        public DayAllocation()
        {
            MealAllocations = new List<MealAllocation>();
        }

        public int Id { get; set; }
        public DayAllocationType Type { get; set; }
        public TargetMacros TargetMacros { get; set; }
        public List<MealAllocation> MealAllocations { get; set; }
    }
}