using System;
using Foundation;

namespace MakeMeMove.iOS.ExtensionMethods
{
	public static class DateTimeExtensions
	{
		public static NSDate ToNSDate(this DateTime date)
		{
			var dtBasis = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);


			return NSDate.FromTimeIntervalSince1970(
				date.ToUniversalTime().Subtract(dtBasis).TotalSeconds);
		}
	}
}

