using System;
using System.Linq;
using CoreGraphics;
using MakeMeMove.iOS.Helpers;
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

            SelectedDateLabel.TextColor = FudistColors.PrimaryColor;
            BackButton.TintColor = UIColor.White;
            


            NavigatePrevious.Image = NavigatePrevious.Image.ApplyTheme(FudistColors.InteractableTextColor);
            var backGestureRecognizer = new UITapGestureRecognizer(RegressDate);
            NavigatePrevious.UserInteractionEnabled = true;
            NavigatePrevious.AddGestureRecognizer(backGestureRecognizer);

            NavigateNext.Image = NavigateNext.Image.ApplyTheme(FudistColors.InteractableTextColor);
            var nextGestureRecognizer = new UITapGestureRecognizer(AdvanceDate);
            NavigateNext.UserInteractionEnabled = true;
            NavigateNext.AddGestureRecognizer(nextGestureRecognizer);
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            UpdateData();

            var statusBarColor = new UIView(new CGRect(0, 0, View.Frame.Width, 20))
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

        private void AdvanceDate()
        {
            if (!_historyDate.HasValue) _historyDate = DateTime.Now.Date;
            if (!_historyDate.Value.Date.Equals(DateTime.Now.Date)) _historyDate = _historyDate.Value.AddDays(1);

            UpdateData();
        }

        private void RegressDate()
        {
            if (!_historyDate.HasValue) _historyDate = DateTime.Now.Date;
            _historyDate = _historyDate.Value.AddDays(-1);

            UpdateData();
        }

        private void UpdateData()
        {
            if (!_historyDate.HasValue) return;

            SelectedDateLabel.Text = _historyDate.Value.ToShortDateString();
            NavigateNext.Hidden = _historyDate.Value.Date.Equals(DateTime.Now.Date);
            (ChildViewControllers.Last() as ExerciseHistoryContainerViewController)?.UpdateData(_historyDate.Value);
        }
    }
}