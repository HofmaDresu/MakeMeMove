using System;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Widget;
using MakeMeMove.Droid.DeviceSpecificImplementations;
using Android.Support.V4.View;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Util;
using Android.Views;
using MacroEatMobile.Core;
using MakeMeMove.Droid.Adapters;
using MakeMeMove.Droid.Fragments;
using MakeMeMove.Droid.Utilities;
using AlertDialog = Android.App.AlertDialog;

namespace MakeMeMove.Droid.Activities
{
    [Activity(Label = "@string/app_name", Icon = "@drawable/icon", MainLauncher = true, ScreenOrientation = ScreenOrientation.Portrait, ConfigurationChanges = ConfigChanges.ScreenSize)]
    public class MainActivity : BaseActivity
    {
        private readonly PermissionRequester _permissionRequester = new PermissionRequester();
        private DrawerLayout _drawer;
        private ActionBarDrawerToggle _toggle;
        private TextView _logInOutText;
        private View _userNameSection;
        private TextView _userNameText;
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
            
            SetContentView(Resource.Layout.Main);

            SupportActionBar.Elevation = 2;

            _drawer = FindViewById<DrawerLayout>(Resource.Id.DrawerLayout);
            _logInOutText = FindViewById<TextView>(Resource.Id.LogInOutText);
            _userNameSection = FindViewById(Resource.Id.UserNameSection);
            _userNameText = FindViewById<TextView>(Resource.Id.UserNameText);
            _scheduleLayout = FindViewById(Resource.Id.ScheduleLayout);
            _exerciseListLayout = FindViewById(Resource.Id.ExerciseListLayout);
            _scheduleText = FindViewById<TextView>(Resource.Id.ScheduleText);
            _scheduleIcon = FindViewById<ImageView>(Resource.Id.ScheduleIcon);
            _exerciseListText = FindViewById<TextView>(Resource.Id.ExerciseListText);
            _exerciseListIcon = FindViewById<ImageView>(Resource.Id.ExerciseListIcon);


            _toggle = new ActionBarDrawerToggle(this, _drawer, Resource.String.DrawerOpenDescription, Resource.String.DrawerCloseDescription);
            _drawer.SetDrawerListener(_toggle);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            FindViewById(Resource.Id.ViewHistoryButton).Click += (sender, args) =>
            {
                _drawer.CloseDrawer(GravityCompat.Start);
                if (Data.UserIsPremium())
                {
                    StartActivity(new Intent(this, typeof(ExerciseHistoryActivity)));
                }
                else if(Data.UserIsSignedIn())
                {
                    new AlertDialog.Builder(this)
                        .SetTitle("Premium Account Needed")
                        .SetMessage("Your current account is not subscribed to Fudist Premium. Please double check your subscription status and try again.")
                        .SetPositiveButton("OK", (o, eventArgs) => { })
                        .Show();
                }
                else
                {
                    new AlertDialog.Builder(this)
                        .SetTitle("Account Needed")
                        .SetMessage("You must sign in as a Fudist Premium user to access your exercise history. Would you like to sign in?")
                        .SetPositiveButton("Yes", (o, eventArgs) => { StartActivity(new Intent(this, typeof(LoginActivity))); })
                        .SetNegativeButton("No", (o, eventArgs) => { })
                        .Show();
                }
            };

            var logInOutButton = FindViewById(Resource.Id.LogInOutButton);
            logInOutButton.Click += (sender, args) =>
            {
                _drawer.CloseDrawer(GravityCompat.Start);
                if (!Data.UserIsSignedIn())
                {
                    StartActivity(new Intent(this, typeof (LoginActivity)));
                }
                else
                {
                    AuthorizationSingleton.GetInstance().ClearPerson(this);
                    Data.SignUserOut();
                    ShowUserSignedOut();
                }
            };

            _permissionRequester.RequestPermissions(this);

            _viewPager = FindViewById<ViewPager>(Resource.Id.ViewPager);
            _viewPager.Adapter = new MainFragmentAdapter(FragmentManager, Data);
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

        protected override async void OnResume()
        {
            base.OnResume();

            if (Data.UserIsSignedIn())
            {
                //HACK: MMM should work with the Android 1.2.0 version of our API
                try
                {
                    await fudistConfigAdapter.Configure(/*PackageManager.GetPackageInfo(PackageName, 0).VersionName*/"1.2.0", "Android", UnifiedAnalytics.GetInstance(this));
                }
                catch (Exception ex)
                {
#if DEBUG
                    Log.Error("exception", ex.Message);
#endif
                }
            }

            
            if (Data.UserPremiumStatusNeedsToBeChecked())
            {
                await AuthorizationSingleton.GetInstance().GetPerson(this, true)
                    .ContinueWith(data =>
                    {
                        if (data.IsCanceled || data.IsFaulted)
                        {
                            return;
                        }
                        var person = data.Result;

                        if (person != null)
                        {
                            Data.SignUserIn(person, AuthorizationSingleton.PersonIsProOrHigherUser(person));
                        }
                    });
            }


            if (!Data.UserIsSignedIn())
            {
                ShowUserSignedOut();
            }
            else
            {
                _logInOutText.Text = "Sign Out";
                _userNameSection.Visibility = ViewStates.Visible;
                _userNameText.Text = Data.GetUserName();
            }
        }

        private void ShowUserSignedOut()
        {
            _logInOutText.Text = "Sign In With Fudist";
            _userNameSection.Visibility = ViewStates.Gone;
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
    }
}

