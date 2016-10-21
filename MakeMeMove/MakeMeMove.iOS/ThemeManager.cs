using System;
using System.Collections.Generic;
using System.Text;
using CoreGraphics;
using UIKit;

namespace MakeMeMove.iOS
{
    public static class ThemeManager
    {
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
