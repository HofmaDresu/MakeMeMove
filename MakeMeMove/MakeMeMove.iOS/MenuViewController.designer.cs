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
        UIKit.UILabel UserNameLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        MakeMeMove.iOS.PrimaryBottomBorderView UserNameSection { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.NSLayoutConstraint UserNameSectionHeight { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (UserNameLabel != null) {
                UserNameLabel.Dispose ();
                UserNameLabel = null;
            }

            if (UserNameSection != null) {
                UserNameSection.Dispose ();
                UserNameSection = null;
            }

            if (UserNameSectionHeight != null) {
                UserNameSectionHeight.Dispose ();
                UserNameSectionHeight = null;
            }
        }
    }
}