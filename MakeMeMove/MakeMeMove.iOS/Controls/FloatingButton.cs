﻿using System;
using CoreGraphics;
using Foundation;
using MakeMeMove.iOS.Helpers;
using UIKit;

namespace MakeMeMove.iOS.Controls
{
	public sealed class FloatingButton : UIButton
	{
		public FloatingButton(string title, string subTitle = null)
		{
			var nsTitle = new NSString(title);
			var size = nsTitle.GetSizeUsingAttributes(new UIStringAttributes { Font = TitleLabel.Font });
			var buttonWidth = size.Width + 25;


			Initialize(title, buttonWidth, subTitle);
			this.TranslatesAutoresizingMaskIntoConstraints = false;
			this.WidthAnchor.ConstraintEqualTo(this.Frame.Width).Active = true;
		}

		private void Initialize(string title, nfloat buttonWidth, string subTitle = null)
		{
			if (string.IsNullOrEmpty(subTitle))
			{
				SetTitle(title, UIControlState.Normal);
				SetTitleColor(Colors.InteractableTextColor, UIControlState.Normal);
				Frame = new CGRect(0, 0, buttonWidth, 40);
			}
			else
			{
				Frame = new CGRect(0, 0, buttonWidth, 60);

				var titleLabel = new UILabel(new CGRect(0, 13, buttonWidth, 21))
				{
					Text = title,
					TextColor = Colors.InteractableTextColor,
					TextAlignment = UITextAlignment.Center
				};
				var subtitleLabel = new UILabel(new CGRect(0, 31, buttonWidth, 21))
				{
					Text = subTitle,
					TextColor = Colors.GrayTextColor,
					Font = UIFont.SystemFontOfSize(12),
					TextAlignment = UITextAlignment.Center
				};
				AddSubviews(titleLabel, subtitleLabel);
			}
			BackgroundColor = UIColor.White;
			Layer.BorderColor = Colors.PrimaryColor.CGColor;
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
			(sender as UIButton).BackgroundColor = Colors.TertiaryColor;
		}

		private void RemoveTouchBackgroundColor(object sender, EventArgs e)
		{
			(sender as UIButton).BackgroundColor = UIColor.White;
		}
	}
}

