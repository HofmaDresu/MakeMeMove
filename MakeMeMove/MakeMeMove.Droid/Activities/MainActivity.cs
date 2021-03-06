﻿using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Widget;
using MakeMeMove.Droid.DeviceSpecificImplementations;
using Android.Support.V4.View;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Views;
using MakeMeMove.Droid.Adapters;
using MakeMeMove.Droid.Fragments;
using Android.Runtime;

namespace MakeMeMove.Droid.Activities
{
    [Activity(Label = "@string/app_name", Icon = "@drawable/icon", RoundIcon = "@drawable/Icon_round", MainLauncher = true, ScreenOrientation = ScreenOrientation.Portrait, ConfigurationChanges = ConfigChanges.ScreenSize)]
    public class MainActivity : BaseActivity
    {
        private readonly PermissionRequester _permissionRequester = new PermissionRequester();
        private DrawerLayout _drawer;
        private ActionBarDrawerToggle _toggle;
        private View _scheduleLayout;
        private View _exerciseListLayout;
        private ViewPager _viewPager;
        private TextView _scheduleText;
        private ImageView _scheduleIcon;
        private TextView _exerciseListText;
        private ImageView _exerciseListIcon;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);

            CreateNotificationChannels();

            SetContentView(Resource.Layout.Main);

            SupportActionBar.Elevation = 2;

            _drawer = FindViewById<DrawerLayout>(Resource.Id.DrawerLayout);
            _scheduleLayout = FindViewById(Resource.Id.ScheduleLayout);
            _exerciseListLayout = FindViewById(Resource.Id.ExerciseListLayout);
            _scheduleText = FindViewById<TextView>(Resource.Id.ScheduleText);
            _scheduleIcon = FindViewById<ImageView>(Resource.Id.ScheduleIcon);
            _exerciseListText = FindViewById<TextView>(Resource.Id.ExerciseListText);
            _exerciseListIcon = FindViewById<ImageView>(Resource.Id.ExerciseListIcon);


            _toggle = new ActionBarDrawerToggle(this, _drawer, Resource.String.DrawerOpenDescription, Resource.String.DrawerCloseDescription);
            _drawer.AddDrawerListener(_toggle);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            FindViewById(Resource.Id.SettingsButton).Click += (sender, args) =>
            {
                _drawer.CloseDrawer(GravityCompat.Start);
                StartActivity(new Intent(this, typeof(SettingsActivity)));
            };

            FindViewById(Resource.Id.ViewTotalsButton).Click += (sender, args) =>
            {
                _drawer.CloseDrawer(GravityCompat.Start);
                StartActivity(new Intent(this, typeof(ExerciseTotalsActivity)));
            };

            FindViewById(Resource.Id.ViewHistoryButton).Click += (sender, args) =>
            {
                _drawer.CloseDrawer(GravityCompat.Start);
                StartActivity(new Intent(this, typeof(ExerciseHistoryActivity)));
            };

            _permissionRequester.RequestPermissions(this);

            _viewPager = FindViewById<ViewPager>(Resource.Id.ViewPager);
            _viewPager.Adapter = new MainFragmentAdapter(SupportFragmentManager, this);
            _viewPager.PageSelected += ViewPager_PageSelected;

            _scheduleLayout.Click += (sender, args) => _viewPager.SetCurrentItem(0, true);
            _exerciseListLayout.Click += (sender, args) => _viewPager.SetCurrentItem(1, true);
        }

        private void ViewPager_PageSelected(object sender, ViewPager.PageSelectedEventArgs e)
        {
            var selectedPage = (_viewPager.Adapter as MainFragmentAdapter).GetItem(e.Position);

            if (selectedPage is ExerciseListFragment)
            {
                _scheduleIcon.SetImageResource(Resource.Drawable.ScheduleUnselected);
                _scheduleText.Typeface = Typeface.Default;
                _scheduleLayout.SetBackgroundResource(Resource.Drawable.MainTabUnselected);
                 
                _exerciseListIcon.SetImageResource(Resource.Drawable.ExerciseListSelected);
                _exerciseListText.Typeface = Typeface.DefaultBold;
                _exerciseListLayout.SetBackgroundResource(Resource.Drawable.MainTabSelected);
            }
            else
            {
                _scheduleIcon.SetImageResource(Resource.Drawable.ScheduleSelected);
                _scheduleText.Typeface = Typeface.DefaultBold;
                _scheduleLayout.SetBackgroundResource(Resource.Drawable.MainTabSelected);

                _exerciseListIcon.SetImageResource(Resource.Drawable.ExerciseListUnselected);
                _exerciseListText.Typeface = Typeface.Default;
                _exerciseListLayout.SetBackgroundResource(Resource.Drawable.MainTabUnselected);
            }
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (_toggle.OnOptionsItemSelected(item))
            {
                return true;
            }
            return base.OnOptionsItemSelected(item);
        }

        protected override void OnPostCreate(Bundle savedInstanceState)
        {
            _toggle.SyncState();
            base.OnPostCreate(savedInstanceState);
        }

        private void CreateNotificationChannels()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                var notificationManager = (NotificationManager)GetSystemService(NotificationService);

                var gameChannel = new NotificationChannel(Constants.ExerciseNotificationChannelId, "Time To Move Notifications", NotificationImportance.Max)
                {
                    Description = "These notifications make you move with an exercise from your list."
                };
                gameChannel.EnableVibration(true);

                var generalChannel = new NotificationChannel(Constants.TodaysProgressNotificationChannelId, "Today's Progress Notifications", NotificationImportance.Low)
                {
                    Description = "These notifications let you know it's time to check your progress for the day."
                };
                generalChannel.EnableVibration(true);

                notificationManager.CreateNotificationChannel(gameChannel);
                notificationManager.CreateNotificationChannel(generalChannel);
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}

