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

namespace MakeMeMove.iOS.ViewControllers
{
    [Register ("MenuViewController")]
    partial class MenuViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView MenuBackgroundView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel OpenFudistLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        MakeMeMove.iOS.PrimaryBottomBorderView OpenFudistView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel SignInOutLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        MakeMeMove.iOS.PrimaryBottomBorderView SignInOutView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel UserNameLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        MakeMeMove.iOS.PrimaryBottomBorderView UserNameView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.NSLayoutConstraint UserNameViewHeightConstraint { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        MakeMeMove.iOS.PrimaryBottomBorderView ViewHistoryView { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (MenuBackgroundView != null) {
                MenuBackgroundView.Dispose ();
                MenuBackgroundView = null;
            }

            if (OpenFudistLabel != null) {
                OpenFudistLabel.Dispose ();
                OpenFudistLabel = null;
            }

            if (OpenFudistView != null) {
                OpenFudistView.Dispose ();
                OpenFudistView = null;
            }

            if (SignInOutLabel != null) {
                SignInOutLabel.Dispose ();
                SignInOutLabel = null;
            }

            if (SignInOutView != null) {
                SignInOutView.Dispose ();
                SignInOutView = null;
            }

            if (UserNameLabel != null) {
                UserNameLabel.Dispose ();
                UserNameLabel = null;
            }

            if (UserNameView != null) {
                UserNameView.Dispose ();
                UserNameView = null;
            }

            if (UserNameViewHeightConstraint != null) {
                UserNameViewHeightConstraint.Dispose ();
                UserNameViewHeightConstraint = null;
            }

            if (ViewHistoryView != null) {
                ViewHistoryView.Dispose ();
                ViewHistoryView = null;
            }
        }
    }
}