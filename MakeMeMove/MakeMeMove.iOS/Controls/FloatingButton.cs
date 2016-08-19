using System;
using CoreGraphics;
using Foundation;
using UIKit;

namespace MakeMeMove.iOS.Controls
{
	public sealed class FloatingButton : UIButton
	{
		private UIView _containerView;
		public FloatingButton(string title, nfloat xPosition, nfloat yPosition, UIView containerView, string subTitle = null)
		{
			var nsTitle = new NSString(title);
			var size = nsTitle.GetSizeUsingAttributes(new UIStringAttributes { Font = TitleLabel.Font });
			var buttonWidth = size.Width + 25;


			Initialize(title, xPosition, yPosition, containerView, buttonWidth, subTitle);
		}

		private void Initialize(string title, nfloat xPosition, nfloat yPosition, UIView containerView, nfloat buttonWidth, string subTitle = null)
		{
			_containerView = containerView;
			if (string.IsNullOrEmpty(subTitle))
			{
				SetTitle(title, UIControlState.Normal);
				SetTitleColor(FudistColors.InteractableTextColor, UIControlState.Normal);
				Frame = new CGRect(xPosition, yPosition, buttonWidth, 40);
			}
			else
			{
				Frame = new CGRect(xPosition, yPosition, buttonWidth, 60);

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

		public void AlignRight(int marginRight = 0)
		{
			Frame = new CGRect(_containerView.Bounds.Width - Frame.Width - marginRight,
				Frame.Y, Frame.Width, Frame.Height);
		}

		public void AlignLeft(int marginLeft = 0)
		{
			Frame = new CGRect(marginLeft,
				Frame.Y, Frame.Width, Frame.Height);
		}
	}
}

