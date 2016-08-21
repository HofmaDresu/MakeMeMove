using System;
using UIKit;

namespace MakeMeMove.iOS.Helpers
{
	public static class FudistColors
	{
		public static UIColor MainBackgroundColor = UIColor.FromRGB(239, 239, 239);
		public static UIColor PrimaryColor = UIColor.FromRGB(215, 78, 10);
		public static UIColor SecondaryColor = UIColor.FromRGBA(215, 78, 10, 127);
		public static UIColor TertiaryColor = UIColor.FromRGB(252, 210, 190);
		public static UIColor InteractableTextColor = UIColor.FromRGB(0, 170, 11);
		public static UIColor WarningColor = UIColor.FromRGB(255, 75, 10);
		public static UIColor GrayTextColor = UIColor.FromRGB(117, 117, 117);
		public static UIColor GrayBorderColor = UIColor.FromRGB(186, 186, 186);

		public static void SetTextPrimaryColor(params UILabel[] labels)
		{
			foreach (var label in labels)
			{
				label.TextColor = PrimaryColor;
			}
		}
	}
}

