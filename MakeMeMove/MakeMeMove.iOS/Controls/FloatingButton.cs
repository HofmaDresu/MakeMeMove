using System;
using CoreGraphics;
using Foundation;
using UIKit;

namespace MakeMeMove.iOS.Controls
{
	public sealed class FloatingButton : UIButton
	{
		private UIView _containerView;
		public FloatingButton(string title, string subTitle = null)
		{
			var nsTitle = new NSString(title);
			var size = nsTitle.GetSizeUsingAttributes(new UIStringAttributes { Font = TitleLabel.Font });
			var buttonWidth = size.Width + 25;


			Initialize(title, buttonWidth, subTitle);
		}

		private void Initialize(string title, nfloat buttonWidth, string subTitle = null)
		{
			if (string.IsNullOrEmpty(subTitle))
			{
				SetTitle(title, UIControlState.Normal);
				SetTitleColor(FudistColors.InteractableTextColor, UIControlState.Normal);
				Frame = new CGRect(0, 0, buttonWidth, 40);
			}
			else
			{
				Frame = new CGRect(0, 0, buttonWidth, 60);

				var titleLabel = new UILabel(new CGRect(0, 13, buttonWidth, 21))
				{
					Text = title,
					TextColor = FudistColors.InteractableTextColor,
					TextAlignment = UITextAlignment.Center
				};
				var subtitleLabel = new UILabel(new CGRect(0, 31, buttonWidth, 21))
				{
					Text = subTitle,
					TextColor = FudistColors.GrayTextColor,
					Font = UIFont.SystemFontOfSize(12),
					TextAlignment = UITextAlignment.Center
				};
				AddSubviews(titleLabel, subtitleLabel);
			}
			BackgroundColor = UIColor.White;
			Layer.BorderColor = FudistColors.PrimaryColor.CGColor;
			Layer.BorderWidth = 1f;
			Layer.CornerRadius = 6f;

			TouchDown += AddTouchBackgroundColor;
			TouchDragEnter += AddTouchBackgroundColor;
			TouchUpInside += RemoveTouchBackgroundColor;
			TouchCancel += RemoveTouchBackgroundColor;
			TouchDragExit += RemoveTouchBackgroundColor;
		}


		private void AddTouchBackgroundColor(object sender, EventArgs e)
		{
			(sender as UIButton).BackgroundColor = FudistColors.TertiaryColor;
		}

		private void RemoveTouchBackgroundColor(object sender, EventArgs e)
		{
			(sender as UIButton).BackgroundColor = UIColor.White;
		}
	}
}

