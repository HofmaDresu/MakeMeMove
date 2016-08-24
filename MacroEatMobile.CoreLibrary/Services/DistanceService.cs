using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MacroEatMobile.Core.Services
{
    public static class DistanceService
    {
        public static double GetDistanceBetweenPoints(double startLat, double startLong, decimal? endLat, decimal? endLong)
        {
            // Default to really far away if we are missing either lat or long
            if (!(endLat.HasValue && endLong.HasValue))
            {
                return 9000;
            }
            return GetDistanceFromLatLonInKm(startLat, startLong, (double)endLat.Value, (double)endLong.Value);
        }

        public static double GetDistanceBetweenPoints(double startLat, double startLong, double endLat, double endLong)
        {
            return GetDistanceFromLatLonInKm(startLat, startLong, endLat, endLong);
        }

        private static double GetDistanceFromLatLonInKm(double startLat, double startLong, double endLat, double endLong) {
          var R = 6371; // Radius of the earth in km
          var dLat = deg2rad(endLat - startLat);  // deg2rad below
          var dLon = deg2rad(endLong - startLong); 
          var a =
            Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
            Math.Cos(deg2rad(startLat)) * Math.Cos(deg2rad(endLat)) *
            Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

          var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a)); 
          var d = R * c; // Distance in km
          return d;
        }

        private static double deg2rad(double deg) {
            return deg * (Math.PI / 180);
        }
    }
}
