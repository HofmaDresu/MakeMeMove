﻿using System;
using CoreGraphics;
using MakeMeMove.iOS.Helpers;
using UIKit;

namespace MakeMeMove.iOS.Controls
{
	public class StatusBarColorUIView : UIView
    {
		public StatusBarColorUIView(nfloat width)
			: base(new CGRect(0, 0, width, GetHeight()))
		{
			BackgroundColor = Colors.PrimaryColor;
		}
        
        private static nfloat GetHeight() 
		{
			var mainWindow = UIApplication.SharedApplication.Delegate.GetWindow();
            return mainWindow.SafeAreaInsets.Top;
        }
    }
}
