using System;
using CoreGraphics;
using Foundation;
using MakeMeMove.iOS.Helpers;
using MakeMeMove.iOS.ViewControllers.Base;
using UIKit;

namespace MakeMeMove.iOS.ViewControllers
{
    public partial class ExerciseHistoryViewController : UIViewController
    {
        private DateTime? _historyDate;

        public ExerciseHistoryViewController (IntPtr handle) : base (handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            _historyDate = _historyDate ?? DateTime.Now.Date;
            View.BackgroundColor = FudistColors.MainBackgroundColor;
            NavBar.Translucent = false;
            NavBar.BarTintColor = FudistColors.PrimaryColor;
            NavBar.TitleTextAttributes = new UIStringAttributes
            {
                ForegroundColor = UIColor.White
            };

            DateDisplayView.BackgroundColor = FudistColors.MainBackgroundColor;

            SelectedDateLabel.Text = _historyDate.Value.ToShortDateString();
            SelectedDateLabel.TextColor = FudistColors.PrimaryColor;
            BackButton.TintColor = UIColor.White;
            NavigateNext.Image = NavigateNext.Image.ApplyTheme(FudistColors.InteractableTextColor);
            NavigatePrevious.Image = NavigatePrevious.Image.ApplyTheme(FudistColors.InteractableTextColor);
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            if (_historyDate.GetValueOrDefault(DateTime.Now.Date) == DateTime.Now.Date)
            {
                NavigateNext.Hidden = true;
            }

            var statusBarColor = new UIView(new CGRect(0, 0, this.View.Frame.Width, 20))
            {
                BackgroundColor = FudistColors.PrimaryColor
            };
            View.Add(statusBarColor);

            var dateViewBottomBorder = new UIView(new CGRect(0, DateDisplayView.Frame.Height -1, View.Frame.Width, 1))
            {
                BackgroundColor = FudistColors.PrimaryColor
            };
            DateDisplayView.Add(dateViewBottomBorder);

            BackButton.Clicked += BackButton_Clicked;
        }

        private void BackButton_Clicked(object sender, EventArgs e)
        {
            DismissViewController(true, () => { });
        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
            BackButton.Clicked -= BackButton_Clicked;
        }
    }
}