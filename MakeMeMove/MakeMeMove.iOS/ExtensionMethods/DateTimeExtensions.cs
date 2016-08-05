using System;
using Foundation;

namespace MakeMeMove.iOS.ExtensionMethods
{
	public static class DateTimeExtensions
	{
		public static NSDate ToNSDate(this DateTime date)
		{
			DateTime reference = TimeZone.CurrentTimeZone.ToLocalTime(
				new DateTime(2001, 1, 1, 0, 0, 0));
			return NSDate.FromTimeIntervalSinceReferenceDate(
				(date - reference).TotalSeconds);
		}
	}
}

