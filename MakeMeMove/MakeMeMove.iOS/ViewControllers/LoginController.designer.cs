// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace MakeMeMove.iOS
{
    [Register ("LoginController")]
    partial class LoginController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIBarButtonItem BackButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton FacebookButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton GoogleButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel MessageLabel { get; set; }

        [Action ("FacebookButton_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void FacebookButton_TouchUpInside (UIKit.UIButton sender);

        [Action ("GoogleButton_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void GoogleButton_TouchUpInside (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (BackButton != null) {
                BackButton.Dispose ();
                BackButton = null;
            }

            if (FacebookButton != null) {
                FacebookButton.Dispose ();
                FacebookButton = null;
            }

            if (GoogleButton != null) {
                GoogleButton.Dispose ();
                GoogleButton = null;
            }

            if (MessageLabel != null) {
                MessageLabel.Dispose ();
                MessageLabel = null;
            }
        }
    }
}