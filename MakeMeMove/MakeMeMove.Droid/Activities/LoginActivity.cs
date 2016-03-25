using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Common;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using MacroEatMobile.Core;
using MacroEatMobile.Core.UtilityInterfaces;
using MakeMeMove.Droid.Utilities;
using Xamarin.Auth;

namespace MakeMeMove.Droid.Activities
{
    [Activity(ScreenOrientation = ScreenOrientation.Portrait,
        WindowSoftInputMode = SoftInput.AdjustResize | SoftInput.StateHidden)]
    public class LoginActivity : BaseActivity
    {
        private TextView _expandFudistLoginButton;
        private TextView _fudistLoginButton;
        private TextView _fudistLoginCancelButton;
        private SignInButton _googleLoginButton;
        private Button _facebookLoginButton;
        private EditText _usernameText;
        private EditText _passwordText;
        private TextView _loginFailureMessage;
        private RelativeLayout _loadingOverlay;
        private RelativeLayout _fudistLoginArea;
        private LinearLayout _socialLoginArea;
        private LinearLayout _mainContentArea;
        private IUnifiedAnalytics _unifiedAnalytics;


        public override bool OnOptionsItemSelected(IMenuItem selectedItem)
        {
            if (selectedItem.ItemId == Android.Resource.Id.Home)
            {
                Finish();
            }
            return true;
        }

        public override void OnBackPressed()
        {
            if (_fudistLoginArea.Visibility == ViewStates.Visible)
            {
                HideFudistLoginSection();
            }
            else
            {
                base.OnBackPressed();
            }
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            ActionBar.SetDisplayHomeAsUpEnabled(false);
            ActionBar.SetHomeButtonEnabled(false);
            SetTitle(Resource.String.log_in_title);

            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
            }

            SetContentView(Resource.Layout.LogIn);

            _mainContentArea = FindViewById<LinearLayout>(Resource.Id.mainContentArea);
            _loadingOverlay = FindViewById<RelativeLayout>(Resource.Id.LoadingOverlay);
            _expandFudistLoginButton = FindViewById<TextView>(Resource.Id.expand_fudist_login_button);
            _fudistLoginArea = FindViewById<RelativeLayout>(Resource.Id.fudistLoginArea);
            _socialLoginArea = FindViewById<LinearLayout>(Resource.Id.socialLoginArea);
            _fudistLoginButton = FindViewById<TextView>(Resource.Id.AuthorizeBtn);
            _fudistLoginCancelButton = FindViewById<TextView>(Resource.Id.CancelButton);
            _loginFailureMessage = FindViewById<TextView>(Resource.Id.loginFailure);
            _usernameText = FindViewById<EditText>(Resource.Id.username);
            _passwordText = FindViewById<EditText>(Resource.Id.password);
            _facebookLoginButton = FindViewById<Button>(Resource.Id.facebook_sign_in_button);
            _googleLoginButton = FindViewById<SignInButton>(Resource.Id.google_sign_in_button);
            _googleLoginButton.SetSize(SignInButton.SizeWide);

            _unifiedAnalytics = UnifiedAnalytics.GetInstance(this);
        }

        protected override void OnPause()
        {
            base.OnPause();
            _fudistLoginButton.Click -= LoginClick;
            _googleLoginButton.Click -= GoogleLoginButtonOnClick;
            _facebookLoginButton.Click -= FacebookLoginButtonOnClick;
            _expandFudistLoginButton.Click -= ExpandFudistLoginButtonOnClick;
            _fudistLoginCancelButton.Click -= FudistLoginCancelButtonOnClick;
            _passwordText.EditorAction -= PasswordTextOnEditorAction;
        }

        protected override void OnResume()
        {
            base.OnResume();
            _fudistLoginButton.Click += LoginClick;
            _googleLoginButton.Click += GoogleLoginButtonOnClick;
            _facebookLoginButton.Click += FacebookLoginButtonOnClick;
            _expandFudistLoginButton.Click += ExpandFudistLoginButtonOnClick;
            _fudistLoginCancelButton.Click += FudistLoginCancelButtonOnClick;
            _passwordText.EditorAction += PasswordTextOnEditorAction;
        }

        private void PasswordTextOnEditorAction(object sender, TextView.EditorActionEventArgs editorActionEventArgs)
        {
            if (editorActionEventArgs.ActionId == ImeAction.Done || editorActionEventArgs.ActionId == ImeAction.Go)
            {
                AttemptFudistLogin();
            }
        }

        private void FudistLoginCancelButtonOnClick(object sender, EventArgs eventArgs)
        {
            HideFudistLoginSection();
        }

        private void HideFudistLoginSection()
        {
            _fudistLoginArea.Visibility = ViewStates.Gone;
            _expandFudistLoginButton.Visibility = ViewStates.Visible;
            _socialLoginArea.Visibility = ViewStates.Visible;
            _loginFailureMessage.Visibility = ViewStates.Gone;
        }

        private void ExpandFudistLoginButtonOnClick(object sender, EventArgs eventArgs)
        {
            _fudistLoginArea.Visibility = ViewStates.Visible;
            _expandFudistLoginButton.Visibility = ViewStates.Gone;
            _socialLoginArea.Visibility = ViewStates.Gone;
        }

        private void FacebookLoginButtonOnClick(object sender, EventArgs eventArgs)
        {
            _loginFailureMessage.Visibility = ViewStates.Gone;
            var auth = new OAuth2Authenticator(
                clientId: "671043732908389",
                scope: "public_profile, email",
                authorizeUrl: new Uri("https://m.facebook.com/dialog/oauth/"),
                redirectUrl: new Uri("https://api.fudist.co/api/ExternalLogin"))
            { AllowCancel = true };

            auth.Completed += (s, e) =>
            {
                SocialLoginComplete(e, "facebook");
            };

            var intent = auth.GetUI(this);
            StartActivity(intent);
        }

        private void GoogleLoginButtonOnClick(object sender, EventArgs eventArgs)
        {
            _loginFailureMessage.Visibility = ViewStates.Gone;
            var auth = new OAuth2Authenticator(
                clientId: "700800275641-m2kon3vbvb0em6bu35qjdqbqnd2qilib.apps.googleusercontent.com",
                scope: "https://www.googleapis.com/auth/userinfo.email",
                authorizeUrl: new Uri("https://accounts.google.com/o/oauth2/auth"),
                redirectUrl: new Uri("https://api.fudist.co/oauth2callback"))
            { AllowCancel = true };

            auth.Completed += (s, e) =>
            {
                SocialLoginComplete(e, "google");
            };

            var intent = auth.GetUI(this);
            StartActivity(intent);
        }

        async void SocialLoginComplete(AuthenticatorCompletedEventArgs e, string socialProvider)
        {
            if (!e.IsAuthenticated) return;

            string accessToken;

            e.Account.Properties.TryGetValue("access_token", out accessToken);
            Person person;
            try
            {
                ShowLoadingOverlay();
                person = await FudistPersonAdapter.AuthPersonWithToken(accessToken, socialProvider, _unifiedAnalytics, AuthorizationSingleton.CreateIAuthorization(this));
            }
            catch (Exception)
            {
                ShowErrorMessage(Resource.String.login_failure);
                HideLoadingOverlayMainScreen();
                return;
            }
            AuthorizationSingleton.GetInstance().SetPerson(person, this);
            Data.SignUserIn(person, AuthorizationSingleton.PersonIsProOrHigherUser(person));
            NavigateToMainScreen();
        }

        private void LoginClick(object sender, EventArgs e)
        {
            AttemptFudistLogin();
        }

        private void AttemptFudistLogin()
        {
            _loginFailureMessage.Visibility = ViewStates.Gone;
            HideKeyboard();
            _fudistLoginButton.Click -= LoginClick;

            ShowLoadingOverlay();
            _mainContentArea.LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, 0, 0);
            FudistPersonAdapter.AuthPerson(_usernameText.Text, _passwordText.Text, _unifiedAnalytics,
                AuthorizationSingleton.CreateIAuthorization(this))
                .ContinueWith(result => RunOnUiThread(() =>
                {
                    if (result.IsCanceled || result.IsFaulted)
                    {
                        _mainContentArea.LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent,
                            ViewGroup.LayoutParams.WrapContent, 0);
                        HideLoadingOverlayFudistLogin();
                        //GeneralDialogs.ShowNetworkErrorDialog(this, false);
                        _fudistLoginButton.Click += LoginClick;
                    }
                    else
                    {
                        if (result.Result == null)
                        {
                            ShowErrorMessage(Resource.String.login_failure);
                            HideLoadingOverlayFudistLogin();
                            _fudistLoginButton.Click += LoginClick;
                            return;
                        }

                        AuthorizationSingleton.GetInstance().SetPerson(result.Result, this);
                        Data.SignUserIn(result.Result, AuthorizationSingleton.PersonIsProOrHigherUser(result.Result));
                        NavigateToMainScreen();
                    }
                }));
        }

        private void ShowErrorMessage(int messageResourceId)
        {
            _loginFailureMessage.Visibility = ViewStates.Visible;
            _loginFailureMessage.Text = Resources.GetString(messageResourceId);
            _mainContentArea.LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.WrapContent, 0);
        }

        private void NavigateToMainScreen()
        {
            var intent = new Intent(this, typeof(MainActivity));
            StartActivity(intent);
        }

        private void HideKeyboard()
        {
            var inputMethodManager = Application.GetSystemService(InputMethodService) as InputMethodManager;
            inputMethodManager?.HideSoftInputFromWindow(_passwordText.WindowToken, HideSoftInputFlags.None);
        }

        private void ShowLoadingOverlay()
        {
            _fudistLoginArea.Visibility = ViewStates.Gone;
            _expandFudistLoginButton.Visibility = ViewStates.Gone;
            _socialLoginArea.Visibility = ViewStates.Gone;
            _loginFailureMessage.Visibility = ViewStates.Gone;
            _loadingOverlay.Visibility = ViewStates.Visible;
        }

        private void HideLoadingOverlayMainScreen()
        {
            _fudistLoginArea.Visibility = ViewStates.Gone;
            _expandFudistLoginButton.Visibility = ViewStates.Visible;
            _socialLoginArea.Visibility = ViewStates.Visible;
            _loadingOverlay.Visibility = ViewStates.Gone;
        }

        private void HideLoadingOverlayFudistLogin()
        {
            _fudistLoginArea.Visibility = ViewStates.Visible;
            _expandFudistLoginButton.Visibility = ViewStates.Gone;
            _socialLoginArea.Visibility = ViewStates.Gone;
            _loadingOverlay.Visibility = ViewStates.Gone;
        }
    }
}