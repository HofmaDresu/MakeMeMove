using Foundation;
using System;
using System.Linq;
using MakeMeMove.iOS.Helpers;
using UIKit;

namespace MakeMeMove.iOS
{
    public partial class PrimaryBottomBorderView : UIControl
    {
        public PrimaryBottomBorderView (IntPtr handle) : base (handle)
        {
            var bottomBorder = new UIView
            {
                BackgroundColor = FudistColors.PrimaryColor,
                TranslatesAutoresizingMaskIntoConstraints = false
            };

            AddSubview(bottomBorder);

            bottomBorder.RightAnchor.ConstraintEqualTo(this.RightAnchor, 0).Active = true;
            bottomBorder.LeftAnchor.ConstraintEqualTo(this.LeftAnchor, 0).Active = true;
            bottomBorder.BottomAnchor.ConstraintEqualTo(this.BottomAnchor).Active = true;
            bottomBorder.HeightAnchor.ConstraintEqualTo(1).Active = true;


            BackgroundColor = FudistColors.MainBackgroundColor;

            var labels = Subviews.OfType<UILabel>().ToArray();
            FudistColors.SetTextPrimaryColor(labels);
        }
    }
}