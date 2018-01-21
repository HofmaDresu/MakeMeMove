using CoreGraphics;
using UIKit;

namespace MakeMeMove.iOS.Helpers
{
	public static class Colors
	{
		public static UIColor MainBackgroundColor = UIColor.FromRGB(239, 239, 239);
		public static UIColor PrimaryColor = UIColor.FromRGB(215, 78, 10);
		public static UIColor SecondaryColor = UIColor.FromRGBA(215, 78, 10, 127);
		public static UIColor TertiaryColor = UIColor.FromRGB(252, 210, 190);
		public static UIColor InteractableTextColor = UIColor.FromRGB(0, 170, 11);
		public static UIColor WarningColor = UIColor.FromRGB(255, 75, 10);
		public static UIColor GrayTextColor = UIColor.FromRGB(117, 117, 117);
		public static UIColor GrayBorderColor = UIColor.FromRGB(186, 186, 186);
        public static UIColor DisabledBackgroundColor = UIColor.FromRGB(220, 220, 220);

        public static void SetTextPrimaryColor(params UILabel[] labels)
		{
			foreach (var label in labels)
			{
				label.TextColor = PrimaryColor;
			}
        }

        public static void SetTextInteractableColor(params UILabel[] labels)
        {
            foreach (var label in labels)
            {
                label.TextColor = InteractableTextColor;
            }
        }

        public static void SetTextPrimaryColor(params PickerUITextField[] textViews)
		{
			foreach (var textView in textViews)
			{
				textView.TextColor = PrimaryColor;
			}
        }

        public static UIImage ApplyTheme(this UIImage image, UIColor color)
        {
            var c = color.CGColor;

            UIGraphics.BeginImageContextWithOptions(image.Size, false, image.CurrentScale);
            var context = UIGraphics.GetCurrentContext();
            var bounds = new CGRect(CGPoint.Empty, image.Size);
            context.SetFillColor(c);
            // translate/flip the graphics context (for transforming from CG* coords to UI* coords
            context.TranslateCTM(0, image.Size.Height);
            context.ScaleCTM(1, -1);
            context.ClipToMask(bounds, image.CGImage);
            context.FillRect(bounds);
            var newImage = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();

            return newImage.ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal);
        }
    }
}

