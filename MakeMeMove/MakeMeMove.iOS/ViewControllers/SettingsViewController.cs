using AudioToolbox;
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
        private NSUserDefaults _defaults;

        public SettingsViewController (IntPtr handle) : base (handle)
        {
            ScreenName = "Settings";
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            _defaults = NSUserDefaults.StandardUserDefaults;
            NavBar.Translucent = false;
            NavBar.BarTintColor = Colors.PrimaryColor;
            NavBar.TitleTextAttributes = new UIStringAttributes
            {
                ForegroundColor = UIColor.White
            };

            var statusBarColor = new UIView(new CGRect(0, 0, View.Frame.Width, 20))
            {
                BackgroundColor = Colors.PrimaryColor
            };
            View.Add(statusBarColor);

            Colors.SetTextInteractableColor(NotificationsSectionHeader);

            BackButton.Clicked += BackButton_Clicked;
            PopulateData();
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            BackButton.TintColor = UIColor.White;

            var soundsList = (from Constants.NotificationSounds suit in Enum.GetValues(typeof(Constants.NotificationSounds)) select suit.Humanize(LetterCasing.Title)).ToList();
            _notificationSoundPicker = MirroredPicker.Create(new PickerModel(soundsList), NotificationSoundPicker, doneAction: NotificationSoundPicker_SaveData, changeAction: NotificationSoundPicker_ValueChanged);

        }

        private void PopulateData()
        {
            var soundString = _defaults.StringForKey(Constants.UserDefaultsNotificationSoundsKey);
            NotificationSoundPicker.Text = soundString;
            _notificationSoundPicker.Select((int)soundString.DehumanizeTo<Constants.NotificationSounds>(), 0, false);
        }

        private void NotificationSoundPicker_SaveData()
        {
            _defaults.SetString(NotificationSoundPicker.Text, Constants.UserDefaultsNotificationSoundsKey);
            _defaults.Synchronize();
            ServiceManager.RestartNotificationServiceIfNeeded();
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