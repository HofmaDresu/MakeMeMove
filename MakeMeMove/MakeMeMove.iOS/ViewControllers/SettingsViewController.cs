﻿using AudioToolbox;
using CoreGraphics;
using Foundation;
using Humanizer;
using MakeMeMove.iOS.Controls;
using MakeMeMove.iOS.Helpers;
using MakeMeMove.iOS.Models;
using MakeMeMove.iOS.ViewControllers.Base;
using System;
using System.Linq;
using UIKit;

namespace MakeMeMove.iOS
{
    public partial class SettingsViewController : BaseViewController
    {
        private UIPickerView _notificationSoundPicker;

        public SettingsViewController (IntPtr handle) : base (handle)
        {
            ScreenName = "Settings";
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            NavBar.Translucent = false;
            NavBar.BarTintColor = FudistColors.PrimaryColor;
            NavBar.TitleTextAttributes = new UIStringAttributes
            {
                ForegroundColor = UIColor.White
            };

            var statusBarColor = new UIView(new CGRect(0, 0, View.Frame.Width, 20))
            {
                BackgroundColor = FudistColors.PrimaryColor
            };
            View.Add(statusBarColor);

            FudistColors.SetTextInteractableColor(NotificationsSectionHeader);

            BackButton.Clicked += BackButton_Clicked;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            BackButton.TintColor = UIColor.White;

            var soundsList = (from Constants.NotificationSounds suit in Enum.GetValues(typeof(Constants.NotificationSounds)) select suit.Humanize(LetterCasing.Title)).ToList();
            _notificationSoundPicker = MirroredPicker.Create(new PickerModel(soundsList), NotificationSoundPicker, doneAction: null, changeAction: NotificationSoundPicker_ValueChanged);

        }

        private void NotificationSoundPicker_ValueChanged()
        {
            var sound = NotificationSoundPicker.Text.DehumanizeTo<Constants.NotificationSounds>();

            if (sound == Constants.NotificationSounds.SystemDefault) return;

            var soundFile = Constants.NotificaitonSoundsMap[sound];
            var soundFileUrl = NSUrl.FromFilename(soundFile);
            var systemSound = new SystemSound(soundFileUrl);
            systemSound.PlaySystemSound();
        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
            BackButton.Clicked -= BackButton_Clicked;
        }

        private void BackButton_Clicked(object sender, EventArgs e)
        {
            DismissViewController(true, () => { });
        }
    }
}