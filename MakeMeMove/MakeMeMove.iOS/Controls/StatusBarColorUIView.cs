using System;
using CoreGraphics;
using MakeMeMove.iOS.Helpers;
using UIKit;

namespace MakeMeMove.iOS.Controls
{
	public class StatusBarColorUIView : UIView
    {
		public StatusBarColorUIView(nfloat width)
			: base(new CGRect(0, 0, width, 20))
		{
			BackgroundColor = Colors.PrimaryColor;
		}
    }
}
