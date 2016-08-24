using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MacroEatMobile.CoreLibrary;
using MacroEatMobile.CoreLibrary.Model;

namespace MacroEatMobile.Core
{
	public static class NutritionService
	{
		private const int DefaultMaxCalories = 2000;
		private const int DefaultMaxFat = 200;
		private const int DefaultMaxCarbs = 500;
        private const int DefaultMaxProtein = 500;
        private static readonly decimal[] ActivityLevelMultipliers = { 1m, 1.2m, 1.38m, 1.55m, 1.73m, 1.9m };

        public static TargetMacros CalculateTargetNutrition(DateTime birthDate, string gender, int weight, int height, int jobActivityLevelIndex, int maxJobActivityLevel,
            int workoutHoursIndex, int maxWorkoutHours, int weightGoalIndex, int nutritionalPlanId)
        {
            var targetMacros = new TargetMacros();

            var weightInKg = weight * 0.453592m;
            var heightInCm = height * 2.54m;

            var today = DateTime.Today;
            var age = today.Year - birthDate.Year;
            if (birthDate.Date > today.AddYears(-age)) age--;

            var calories = (9.99m * weightInKg) + (6.25m * heightInCm) - (4.92m * age);

            if (gender.ToLower().StartsWith("m"))
            {
                calories += 5;
            }
            else
            {
                calories -= 161;
            }

            var activityLevel = CalculateActivityLevel(jobActivityLevelIndex, maxJobActivityLevel, workoutHoursIndex, maxWorkoutHours);

            calories *= activityLevel;

            calories = CaloriesAdjustedByWeightGoal(weightGoalIndex, calories);

            targetMacros.Calories = (int)Math.Floor(calories);

            targetMacros = CalculateMacrosForNutritionalPlan(nutritionalPlanId, targetMacros);

            return targetMacros;
        }

        private static TargetMacros CalculateMacrosForNutritionalPlan(int nutritionalPlanId, TargetMacros inputMacros)
        {
            var macros = new TargetMacros
            {
                Calories = inputMacros.Calories
            };

            switch (nutritionalPlanId)
            {
                case 0:
                    // Standard
                    // source http://iifym.com/iifym-calculator/
                    macros.Carbohydrates = GeneralUtilities.RoundToNearest<int>(GetCarbohydrateGramsFromCalories(macros.Calories * 0.50m), 5);
                    macros.Fat = GeneralUtilities.RoundToNearest<int>(GetFatGramsFromCalories(macros.Calories * 0.25m), 5);
                    macros.Protein = GeneralUtilities.RoundToNearest<int>(GetProteinGramsFromCalories(macros.Calories * 0.25m), 5);
                    break;
                case 1:
                    // Body Builder
                    // source http://iifym.com/iifym-calculator/
                    macros.Carbohydrates = GeneralUtilities.RoundToNearest<int>(GetCarbohydrateGramsFromCalories(macros.Calories * 0.40m), 5);
                    macros.Fat = GeneralUtilities.RoundToNearest<int>(GetFatGramsFromCalories(macros.Calories * 0.20m), 5);
                    macros.Protein = GeneralUtilities.RoundToNearest<int>(GetProteinGramsFromCalories(macros.Calories * 0.40m), 5);
                    break;
                case 2:
                    // Low Carb
                    // source http://iifym.com/iifym-calculator/
                    macros.Carbohydrates = GeneralUtilities.RoundToNearest<int>(GetCarbohydrateGramsFromCalories(macros.Calories * 0.25m), 5);
                    macros.Fat = GeneralUtilities.RoundToNearest<int>(GetFatGramsFromCalories(macros.Calories * 0.35m), 5);
                    macros.Protein = GeneralUtilities.RoundToNearest<int>(GetProteinGramsFromCalories(macros.Calories * 0.40m), 5);
                    break;
                case 3:
                    // Low Fat
                    // source http://iifym.com/iifym-calculator/
                    macros.Carbohydrates = GeneralUtilities.RoundToNearest<int>(GetCarbohydrateGramsFromCalories(macros.Calories * 0.60m), 5);
                    macros.Fat = GeneralUtilities.RoundToNearest<int>(GetFatGramsFromCalories(macros.Calories * 0.15m), 5);
                    macros.Protein = GeneralUtilities.RoundToNearest<int>(GetProteinGramsFromCalories(macros.Calories * 0.25m), 5);
                    break;
                case 4:
                    // Zone Diet
                    // source http://iifym.com/iifym-calculator/
                    macros.Carbohydrates = GeneralUtilities.RoundToNearest<int>(GetCarbohydrateGramsFromCalories(macros.Calories * 0.40m), 5);
                    macros.Fat = GeneralUtilities.RoundToNearest<int>(GetFatGramsFromCalories(macros.Calories * 0.30m), 5);
                    macros.Protein = GeneralUtilities.RoundToNearest<int>(GetProteinGramsFromCalories(macros.Calories * 0.30m), 5);
                    break;
                case 5:
                    // Ketogenic
                    // source http://www.ketogenic-diet-resource.com/ketogenic-diet-plan.html
                    macros.Carbohydrates = GeneralUtilities.RoundToNearest<int>(GetCarbohydrateGramsFromCalories(macros.Calories * 0.05m), 5);
                    macros.Fat = GeneralUtilities.RoundToNearest<int>(GetFatGramsFromCalories(macros.Calories * 0.75m), 5);
                    macros.Protein = GeneralUtilities.RoundToNearest<int>(GetProteinGramsFromCalories(macros.Calories * 0.20m), 5);
                    break;
                case 6:
                    // Custom
                    macros.Carbohydrates = GeneralUtilities.RoundToNearest<int>(GetCarbohydrateGramsFromCalories(macros.Calories * 0.35m), 5);
                    macros.Fat = GeneralUtilities.RoundToNearest<int>(GetFatGramsFromCalories(macros.Calories * 0.35m), 5);
                    macros.Protein = GeneralUtilities.RoundToNearest<int>(GetProteinGramsFromCalories(macros.Calories * 0.30m), 5);
                    break;
                default:
                    throw new Exception("Nutritional Plan Id out of range");
            }

            //Adjust calories to match rounded macros
            macros.Calories = GetCaloriesFromMacros(macros.Fat, macros.Protein, macros.Carbohydrates);
            return macros;
        }

        private static decimal CaloriesAdjustedByWeightGoal(int weightGoalIndex, decimal calories)
        {
            switch (weightGoalIndex)
            {
                case 0:
                    // Lose Weight (Aggressive)
                    return Math.Max(calories - (calories*.2m), 0m);
                case 1:
                    // Lose Weight (Cautious)
                    return Math.Max(calories - (calories * .15m), 0m);
                case 2:
                    // Maintain
                    return calories;
                case 3:
                    // Bulk Up (Cautious)
                    return Math.Max(calories + (calories *.05m), 0m);
                case 4:
                    // Bulk Up (Aggressive)
                    return Math.Max(calories + (calories * .1m), 0m);
                default:
                    throw new Exception("Weight Goal Index out of range");
            }
        }

        private static decimal CalculateActivityLevel(int jobActivityLevelIndex, int maxJobActivityLevel, int workoutHoursIndex, int maxWorkoutHours)
        {
            var jobActivityLevel = GetJobActivityLevel(jobActivityLevelIndex, maxJobActivityLevel);
            var workoutHoursActivityLevel = GetWorkoutHoursActivityLevel(workoutHoursIndex, maxWorkoutHours);
            var activityLevel = ActivityLevelMultipliers[(int)Math.Floor((jobActivityLevel + workoutHoursActivityLevel) / 2d)];
            return activityLevel;
        }

        private static int GetJobActivityLevel(int jobActivityLevelIndex, int maxJobActivityLevel)
        {
            var jobActivityLevelRange = jobActivityLevelIndex / (double)maxJobActivityLevel;
            if (jobActivityLevelRange < 1d / 6d)
            {
                return 0;
            }
            else if (jobActivityLevelRange < 2d / 6d)
            {
                return 1;
            }
            else if (jobActivityLevelRange < 3d / 6d)
            {
                return 2;
            }
            else if (jobActivityLevelRange < 4d / 6d)
            {
                return 3;
            }
            else if (jobActivityLevelRange < 5d / 6d)
            {
                return 4;
            }
            else
            {
                return 5;
            }
        }

        private static int GetWorkoutHoursActivityLevel(int workoutHoursIndex, int maxWorkoutHours)
        {
            var workoutHoursRange = workoutHoursIndex / (double)maxWorkoutHours;
            if (workoutHoursRange < 1d / 6d)
            {
                return 0;
            }
            else if (workoutHoursRange < 2d / 6d)
            {
                return 1;
            }
            else if (workoutHoursRange < 3d / 6d)
            {
                return 2;
            }
            else if (workoutHoursRange < 4d / 6d)
            {
                return 3;
            }
            else if (workoutHoursRange < 5d / 6d)
            {
                return 4;
            }
            else
            {
                return 5;
            }
        }
        
        public static int GetCaloriesFromMacros(int fatGrams, int proteinGrams, int carbGrams)
        {
            return (9 * fatGrams) + (4 * proteinGrams) + (4 * carbGrams);
        }

		public static T GetMaxCalories<T>()
		{
			return (T)Convert.ChangeType(DefaultMaxCalories, typeof(T));
		}

		public static T GetMaxFat<T>()
		{
			return (T)Convert.ChangeType(DefaultMaxFat, typeof(T));
		}

		public static T GetMaxProtein<T>()
		{
			return (T)Convert.ChangeType(DefaultMaxProtein, typeof(T));
		}

		public static T GetMaxCarbohydrates<T>()
		{
			return (T)Convert.ChangeType(DefaultMaxCarbs, typeof(T));
        }

        public static T GetDefaultFatGrams<T>()
        {
            return (T)Convert.ChangeType(56, typeof(T));
        }

        public static T GetDefaultProteinGrams<T>()
        {
            return (T)Convert.ChangeType(126, typeof(T));
        }

        public static T GetDefaultCarbohydratesGrams<T>()
        {
            return (T)Convert.ChangeType(253, typeof(T));
        }

		public static Tuple<float, float> GetAdjustedCalorieRange(Person person, DayAllocationType dayType, int? selectedMeal = null, decimal? calsAlreadyConsumed = null)
        {
            var dayAllocation = person.DayAllocations.SingleOrDefault(da => da.Type == dayType);

            var personCalories = dayAllocation?.TargetMacros.Calories ?? DefaultMaxCalories;

			decimal? thisMealCals;

			if (calsAlreadyConsumed != null) {
				var remainingMealsPercentage = person.DayAllocations.Single(da => da.Type == dayType).MealAllocations.Skip (selectedMeal ?? 0).Sum (ma => ma.DailyPercent);
				var percentOfRemaining = person.DayAllocations.Single(da => da.Type == dayType).MealAllocations[selectedMeal ?? 0].DailyPercent / (decimal)remainingMealsPercentage;
				var remainingCals = person.DayAllocations.Single(da => da.Type == dayType).TargetMacros.Calories - calsAlreadyConsumed;

				thisMealCals = remainingCals * percentOfRemaining;
			} else {
				thisMealCals = (person.DayAllocations.Single(da => da.Type == dayType).TargetMacros.Calories * person.DayAllocations.Single(da => da.Type == dayType).MealAllocations[selectedMeal ?? 0].DailyPercent / 100m);
			}

			return GetMacroRange(dayAllocation, thisMealCals.Value, DefaultMaxCalories, 500, 800, selectedMeal, 50);
		}

        public static Tuple<float, float> GetAdjustedFatRange(Person person, DayAllocationType dayType, int? selectedMeal)
        {
            var dayAllocation = person.DayAllocations.SingleOrDefault(da => da.Type == dayType);

            decimal personFat = dayAllocation?.TargetMacros.Fat ?? DefaultMaxFat;

			if ( selectedMeal.HasValue )
				personFat *= (dayAllocation?.MealAllocations[selectedMeal.Value].DailyPercent ?? 100 / 100m);

            return GetMacroRange(dayAllocation, personFat, DefaultMaxFat, 20, 30, selectedMeal, 5);
		}

        public static Tuple<float, float> GetAdjustedProteinRange(Person person, DayAllocationType dayType, int? selectedMeal)
        {
            var dayAllocation = person.DayAllocations.SingleOrDefault(da => da.Type == dayType);

            decimal maxPersonProtein = dayAllocation?.TargetMacros.Protein ?? DefaultMaxProtein;

			if ( selectedMeal.HasValue )
				maxPersonProtein *= (dayAllocation?.MealAllocations[selectedMeal.Value].DailyPercent ?? 100 / 100m);

            return GetMacroRange(dayAllocation, maxPersonProtein, DefaultMaxProtein, 30, 100, selectedMeal, 5);
		}

        public static Tuple<float, float> GetAdjustedCarbohydrateRange(Person person, DayAllocationType dayType, int? selectedMeal)
		{
            var dayAllocation = person.DayAllocations.SingleOrDefault(da => da.Type == dayType);

			decimal personCarbs = dayAllocation?.TargetMacros.Carbohydrates ?? DefaultMaxCarbs;

			if ( selectedMeal.HasValue )
				personCarbs *= (dayAllocation?.MealAllocations[selectedMeal.Value].DailyPercent ?? 100 / 100m);

            return GetMacroRange(dayAllocation, personCarbs, DefaultMaxCarbs, 30, 100, selectedMeal, 5);
		}

		private static Tuple<float, float> GetMacroRange(DayAllocation dayAllocation, decimal personMacro, int defaultMaxMacro, int defaultLeftMacro, int defaultRightMacro, int? selectedMeal, int stepSize)
		{
			Tuple<float, float> macroRange;
			if (selectedMeal.HasValue)
			{
				//the following math should be done before calling into this method hence commented out
                //var dailyPercent = dayAllocation.MealAllocations[selectedMeal.Value].DailyPercent / 100m;
				macroRange = new Tuple<float, float>(
					Math.Min(defaultMaxMacro, GeneralUtilities.RoundToNearest<float>(.8m * personMacro, stepSize)),
					Math.Min(defaultMaxMacro, GeneralUtilities.RoundToNearest<float>(1.2m * personMacro, stepSize)));

			}
			else
			{
				macroRange = new Tuple<float, float>(
					GeneralUtilities.RoundToNearest<float>(defaultLeftMacro, stepSize),
					GeneralUtilities.RoundToNearest<float>(defaultRightMacro, stepSize));
			}
			return macroRange;
		}

        public static Tuple<int, int> GetFatPercentRange(Person person, DayAllocationType dayType)
        {
            var dayAllocation = person.DayAllocations.SingleOrDefault(da => da.Type == dayType);
            var fatPercent = GetFatPercent(dayAllocation);
            return new Tuple<int, int>(GeneralUtilities.RoundToNearest<int>(fatPercent * .8m, 1), GeneralUtilities.RoundToNearest<int>(fatPercent * 1.2m, 1));
        }

        public static decimal GetFatPercent(DayAllocation dayAllocation)
        {
            var fatPercent = 25m;
            if (dayAllocation != null)
            {
                fatPercent = GetCaloriesFromFat(dayAllocation.TargetMacros.Fat) / (decimal)dayAllocation.TargetMacros.Calories;
                fatPercent *= 100;
            }
            return GeneralUtilities.RoundToNearest<int>(fatPercent, 1);
        }

	    public static Tuple<int, int> GetProteinPercentRange(Person person, DayAllocationType dayType)
        {
            var dayAllocation = person.DayAllocations.SingleOrDefault(da => da.Type == dayType);
            var proteinPercent = GetProteinPercent(dayAllocation);
            return new Tuple<int, int>(GeneralUtilities.RoundToNearest<int>(proteinPercent * .8m, 1), GeneralUtilities.RoundToNearest<int>(proteinPercent * 1.2m, 1));
        }

        public static int GetProteinPercent(DayAllocation dayAllocation)
        {
            var proteinPercent = 25m;
            if (dayAllocation != null)
            {
                proteinPercent = GetCaloriesFromProtein(dayAllocation.TargetMacros.Protein) / (decimal)dayAllocation.TargetMacros.Calories;
                proteinPercent *= 100;
            }
            return GeneralUtilities.RoundToNearest<int>(proteinPercent, 1);
        }

	    public static Tuple<int, int> GetCarbyhydratePercentRange(Person person, DayAllocationType dayType)
        {
            var dayAllocation = person.DayAllocations.SingleOrDefault(da => da.Type == dayType);
            var carbPercent = GetCarbyhydratePercent(dayAllocation);
            return new Tuple<int, int>(GeneralUtilities.RoundToNearest<int>(carbPercent * .8m, 1), GeneralUtilities.RoundToNearest<int>(carbPercent * 1.2m, 1));
        }

        public static int GetCarbyhydratePercent(DayAllocation dayAllocation)
        {
            var carbPercent = 50m;
            if (dayAllocation != null)
            {
                carbPercent = GetCaloriesFromCarbohydrates(dayAllocation.TargetMacros.Carbohydrates) / (decimal)dayAllocation.TargetMacros.Calories;
                carbPercent *= 100;
            }
            return GeneralUtilities.RoundToNearest<int>(carbPercent, 1);
        }

        private static int GetCaloriesFromFat(int fatGrams)
        {
            return (fatGrams * 9);
        }

        private static int GetCaloriesFromProtein(int proteinGrams)
        {
            return (proteinGrams * 4);
        }

	    private static int GetCaloriesFromCarbohydrates(int carbohydrateGrams)
	    {
	        return (carbohydrateGrams * 4);
	    }

        private static decimal GetFatGramsFromCalories(decimal calories)
        {
            return calories / 9m;
        }

        private static decimal GetProteinGramsFromCalories(decimal calories)
        {
            return calories / 4m;
        }

        private static decimal GetCarbohydrateGramsFromCalories(decimal calories)
        {
            return calories / 4m;
        }
        
        public static RegistrationSimpleDayAllocation GetDefaultMealAllocations(int numberOfMeals, bool hasSnack, TargetMacros macrosForDay, 
            string breakfastName, string brunchName, string lunchName, string dinnerName, string supperName, string snackName)
        {
            var dayAllocation = new RegistrationSimpleDayAllocation
            {
                TotalMacros = macrosForDay
            };

            switch (numberOfMeals)
            {
                case 1:
                    if (hasSnack) throw new InvalidDataException("Snacks are not supported for 1 meal");
                    dayAllocation.SimpleMealAllocations = new List<RegistrationSimpleMealAllocation>
                    {
                        new RegistrationSimpleMealAllocation { MealName = dinnerName, MealSize = SettingsMealSize.VeryLarge}
                    };
                    return dayAllocation;
                case 2:
                    if (hasSnack)
                    {
                        dayAllocation.SimpleMealAllocations = new List<RegistrationSimpleMealAllocation>
                        {
                            new RegistrationSimpleMealAllocation { MealName = breakfastName, MealSize = SettingsMealSize.Large},
                            new RegistrationSimpleMealAllocation { MealName = snackName, MealSize = SettingsMealSize.VerySmall},
                            new RegistrationSimpleMealAllocation { MealName = dinnerName, MealSize = SettingsMealSize.Large}
                        };
                    }
                    else
                    {
                        dayAllocation.SimpleMealAllocations = new List<RegistrationSimpleMealAllocation>
                        {
                            new RegistrationSimpleMealAllocation { MealName =breakfastName, MealSize = SettingsMealSize.Large},
                            new RegistrationSimpleMealAllocation { MealName =dinnerName, MealSize = SettingsMealSize.Large}
                        };
                    }
                    return dayAllocation;
                case 3:
                    if (hasSnack)
                    {
                        dayAllocation.SimpleMealAllocations = new List<RegistrationSimpleMealAllocation>
                        {
                            new RegistrationSimpleMealAllocation { MealName = breakfastName, MealSize = SettingsMealSize.Small},
                            new RegistrationSimpleMealAllocation { MealName = lunchName, MealSize = SettingsMealSize.Medium},
                            new RegistrationSimpleMealAllocation { MealName = snackName, MealSize = SettingsMealSize.VerySmall},
                            new RegistrationSimpleMealAllocation { MealName = dinnerName, MealSize = SettingsMealSize.Large}
                        };
                    }
                    else
                    {
                        dayAllocation.SimpleMealAllocations = new List<RegistrationSimpleMealAllocation>
                        {
                            new RegistrationSimpleMealAllocation { MealName = breakfastName, MealSize = SettingsMealSize.Small},
                            new RegistrationSimpleMealAllocation { MealName = lunchName, MealSize = SettingsMealSize.Medium},
                            new RegistrationSimpleMealAllocation { MealName = dinnerName, MealSize = SettingsMealSize.Large}
                        };
                    }
                    return dayAllocation;
                case 4:
                    if (hasSnack)
                    {
                        dayAllocation.SimpleMealAllocations = new List<RegistrationSimpleMealAllocation>
                        {
                            new RegistrationSimpleMealAllocation { MealName = breakfastName, MealSize = SettingsMealSize.Small},
                            new RegistrationSimpleMealAllocation { MealName = lunchName, MealSize = SettingsMealSize.Medium},
                            new RegistrationSimpleMealAllocation { MealName = snackName, MealSize = SettingsMealSize.VerySmall},
                            new RegistrationSimpleMealAllocation { MealName = dinnerName, MealSize = SettingsMealSize.Medium},
                            new RegistrationSimpleMealAllocation { MealName = supperName, MealSize = SettingsMealSize.Small}
                        };
                    }
                    else
                    {
                        dayAllocation.SimpleMealAllocations = new List<RegistrationSimpleMealAllocation>
                        {
                            new RegistrationSimpleMealAllocation { MealName = breakfastName, MealSize = SettingsMealSize.Small},
                            new RegistrationSimpleMealAllocation { MealName = lunchName, MealSize = SettingsMealSize.Medium},
                            new RegistrationSimpleMealAllocation { MealName = dinnerName, MealSize = SettingsMealSize.Small},
                            new RegistrationSimpleMealAllocation { MealName = supperName, MealSize = SettingsMealSize.Large}
                        };
                    }
                    return dayAllocation;
                case 5:
                    if (hasSnack)
                    {
                        dayAllocation.SimpleMealAllocations = new List<RegistrationSimpleMealAllocation>
                        {
                            new RegistrationSimpleMealAllocation { MealName = breakfastName, MealSize = SettingsMealSize.Small},
                            new RegistrationSimpleMealAllocation { MealName = brunchName, MealSize = SettingsMealSize.Small},
                            new RegistrationSimpleMealAllocation { MealName = lunchName, MealSize = SettingsMealSize.Small},
                            new RegistrationSimpleMealAllocation { MealName = snackName, MealSize = SettingsMealSize.VerySmall},
                            new RegistrationSimpleMealAllocation { MealName = dinnerName, MealSize = SettingsMealSize.Medium},
                            new RegistrationSimpleMealAllocation { MealName = supperName, MealSize = SettingsMealSize.Small}
                        };
                    }
                    else
                    {
                        dayAllocation.SimpleMealAllocations = new List<RegistrationSimpleMealAllocation>
                        {
                            new RegistrationSimpleMealAllocation { MealName = breakfastName, MealSize = SettingsMealSize.Small},
                            new RegistrationSimpleMealAllocation { MealName = brunchName, MealSize = SettingsMealSize.Small},
                            new RegistrationSimpleMealAllocation { MealName = lunchName, MealSize = SettingsMealSize.Small},
                            new RegistrationSimpleMealAllocation { MealName = dinnerName, MealSize = SettingsMealSize.Medium},
                            new RegistrationSimpleMealAllocation { MealName = supperName, MealSize = SettingsMealSize.Medium}
                        };
                    }
                    return dayAllocation;
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                case 12:
                    if (hasSnack) throw new InvalidDataException("Snacks are not supported for 6 or more meals");

                    dayAllocation.SimpleMealAllocations = new List<RegistrationSimpleMealAllocation>();

                    bool breakfastIsUsed = false,
                        brunchIsUsed = false,
                        lunchIsUsed = false,
                        dinnerIsUsed = false,
                        supperIsUsed = false;
                    var numberOfSnacksUsed = 0;

                    for (double i = 1; i <= numberOfMeals; i++)
                    {
                        string mealName;
                        const int numberOfNamedMeals = 5;
                        if (i / numberOfMeals < 1d / numberOfNamedMeals && !breakfastIsUsed)
                        {
                            mealName = breakfastName;
                            breakfastIsUsed = true;
                        }
                        else if (i / numberOfMeals < 2d / numberOfNamedMeals && i / numberOfMeals >= 1d / numberOfNamedMeals && !brunchIsUsed)
                        {
                            mealName = brunchName;
                            brunchIsUsed = true;
                        }
                        else if (i / numberOfMeals < 3d / numberOfNamedMeals && i / numberOfMeals >= 2d / numberOfNamedMeals && !lunchIsUsed)
                        {
                            mealName = lunchName;
                            lunchIsUsed = true;
                        }
                        else if (i / numberOfMeals < 4d / numberOfNamedMeals && i / numberOfMeals >= 3d / numberOfNamedMeals && !dinnerIsUsed)
                        {
                            mealName = dinnerName;
                            dinnerIsUsed = true;
                        }
                        else if (i / numberOfMeals <= 1 && i / numberOfMeals >= 4d / numberOfNamedMeals && !supperIsUsed)
                        {
                            mealName = supperName;
                            supperIsUsed = true;
                        }
                        else
                        {
                            mealName = (numberOfMeals == 6) ? snackName : snackName + " " + ++numberOfSnacksUsed;
                        }

                        dayAllocation.SimpleMealAllocations.Add(
                            new RegistrationSimpleMealAllocation
                            {
                                MealName = mealName,
                                MealSize = SettingsMealSize.Small
                            });
                    }
                    return dayAllocation;
                default:
                    throw new InvalidDataException("We only allow between 1 and 12 meals, inclusively");
            }
        }
	}
}

