using CoreGraphics;
using MakeMeMove.iOS.Helpers;
using UIKit;

namespace MakeMeMove.iOS
{
    public class LoadingOverlay : UIView
    {
        public LoadingOverlay(CGRect frame, string loadingText = "Loading Data...") : base(frame)
        {
            // configurable bits
            BackgroundColor = Colors.MainBackgroundColor;
            //Alpha = 0.75f;
            AutoresizingMask = UIViewAutoresizing.FlexibleDimensions;

            const int labelHeight = 22;
            var labelWidth = Frame.Width - 20;

            // derive the center x and y
            var centerX = Frame.Width / 2;
            var centerY = Frame.Height / 2;

            // create the activity spinner, center it horizontall and put it 5 points above center x
            var activitySpinner = new UIActivityIndicatorView(UIActivityIndicatorViewStyle.WhiteLarge)
            {
                Color = Colors.GrayTextColor
            };
            activitySpinner.Frame = new CGRect(
                centerX - activitySpinner.Frame.Width / 2,
                centerY - activitySpinner.Frame.Height - 20,
                activitySpinner.Frame.Width,
                activitySpinner.Frame.Height);
            activitySpinner.AutoresizingMask = UIViewAutoresizing.FlexibleMargins;
            AddSubview(activitySpinner);
            activitySpinner.StartAnimating();

            // create and configure the "Loading Data" label
            var loadingLabel = new UILabel(new CGRect(
                centerX - labelWidth/2,
                centerY + 20,
                labelWidth,
                labelHeight
                ))
            {
                BackgroundColor = UIColor.Clear,
                TextColor = Colors.GrayTextColor,
                Text = loadingText,
                TextAlignment = UITextAlignment.Center,
                AutoresizingMask = UIViewAutoresizing.FlexibleMargins
            };
            AddSubview(loadingLabel);
        }

        /// <summary>
        /// Fades out the control and then removes it from the super view
        /// </summary>
        public void Hide()
        {
            Animate(
                0.5, // duration
                () => { Alpha = 0; },
                RemoveFromSuperview
            );
        }
    }
}
