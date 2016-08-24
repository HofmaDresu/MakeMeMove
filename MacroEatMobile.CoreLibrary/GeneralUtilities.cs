using System;
using System.Collections.Generic;
using System.Text;

namespace MacroEatMobile.CoreLibrary
{
    public static class GeneralUtilities
    {
        public static T RoundToNearest<T>(decimal fullNumber, int step)
        {
            var shrunkNumber = fullNumber / step;
            var roundedNumber = (int)Math.Ceiling(shrunkNumber);
            return (T)Convert.ChangeType(roundedNumber * step, typeof(T));
        }

        public static string ToString(this decimal number, int decimalPlaces)
        {
            return string.Format(GetFormatString(decimalPlaces), number);
        }
        public static string ToString(this decimal? number, int decimalPlaces)
        {
            return string.Format(GetFormatString(decimalPlaces), number);
        }

        private static string GetFormatString(int decimalPlaces)
        {
            if (decimalPlaces == 0)
            {
                return "{0:0}";
            }
            
            var decimalPlaceFormat = new string('#', decimalPlaces);
            return "{0:0." + decimalPlaceFormat + "}";
        }
    }
}
