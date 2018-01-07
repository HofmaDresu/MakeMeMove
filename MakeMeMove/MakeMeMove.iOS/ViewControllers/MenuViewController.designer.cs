// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
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
        MakeMeMove.iOS.PrimaryBottomBorderView SettingsView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        MakeMeMove.iOS.PrimaryBottomBorderView ViewHistoryView { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (MenuBackgroundView != null) {
                MenuBackgroundView.Dispose ();
                MenuBackgroundView = null;
            }

            if (SettingsView != null) {
                SettingsView.Dispose ();
                SettingsView = null;
            }

            if (ViewHistoryView != null) {
                ViewHistoryView.Dispose ();
                ViewHistoryView = null;
            }
        }
    }
}