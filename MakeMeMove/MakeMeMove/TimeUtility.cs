using System;

namespace MakeMeMove
{
    public static class TimeUtility
    {
        public static int GetCivilianModifiedStartHour(DateTime dateTime)
        {
            if (dateTime.Hour == 12 || dateTime.Hour == 0)
            {
                return 12;
            }
            else if (dateTime.Hour > 12)
            {
                return dateTime.Hour - 12;
            }
            else
            {
                return dateTime.Hour;
            }
        }
    }
}
