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
    [Register ("MenuViewController")]
    partial class MenuViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView MenuBackgroundView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel UserNameLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        MakeMeMove.iOS.PrimaryBottomBorderView UserNameView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        MakeMeMove.iOS.PrimaryBottomBorderView ViewHistoryView { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (MenuBackgroundView != null) {
                MenuBackgroundView.Dispose ();
                MenuBackgroundView = null;
            }

            if (UserNameLabel != null) {
                UserNameLabel.Dispose ();
                UserNameLabel = null;
            }

            if (UserNameView != null) {
                UserNameView.Dispose ();
                UserNameView = null;
            }

            if (ViewHistoryView != null) {
                ViewHistoryView.Dispose ();
                ViewHistoryView = null;
            }
        }
    }
}