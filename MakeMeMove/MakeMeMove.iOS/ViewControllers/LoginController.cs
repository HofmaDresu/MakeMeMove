using System;
using CoreGraphics;
using MacroEatMobile.Core;
using MakeMeMove.iOS.Controls;
using MakeMeMove.iOS.Helpers;
using MakeMeMove.iOS.Utilities;
using MakeMeMove.iOS.ViewControllers.Base;
using UIKit;
using Xamarin.Auth;

namespace MakeMeMove.iOS
{
    public partial class LoginController : BaseViewController
    {
        private FloatingButton _loginWithFudist;
        private LoadingOverlay _loadingOverlay;

        public LoginController (IntPtr handle) : base (handle)
        {
            ScreenName = "Sign In";
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            NavigationController.NavigationBar.Translucent = false;
            NavigationController.NavigationBar.BarTintColor = FudistColors.PrimaryColor;
            NavigationController.NavigationBar.TitleTextAttributes = new UIStringAttributes
            {
                ForegroundColor = UIColor.White
            };
            BackButton.TintColor = UIColor.White;

            ClearMessageText();

            SetupLoginButton();
        }

        private void SetupLoginButton()
        {
            _loginWithFudist = new FloatingButton("Sign In with Fudist");
            _loginWithFudist.TouchUpInside += LoginWithFudistOnTouchUpInside;
            _loginWithFudist.TranslatesAutoresizingMaskIntoConstraints = false;
            View.Add(_loginWithFudist);

            View.AddConstraint(NSLayoutConstraint.Create(_loginWithFudist, NSLayoutAttribute.Top, NSLayoutRelation.Equal,
                GoogleButton, NSLayoutAttribute.Bottom, 1, 20));
            View.AddConstraint(NSLayoutConstraint.Create(_loginWithFudist, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal,
                GoogleButton, NSLayoutAttribute.CenterX, 1, 0));
            View.AddConstraint(NSLayoutConstraint.Create(_loginWithFudist, NSLayoutAttribute.Width, NSLayoutRelation.Equal,
                null, NSLayoutAttribute.NoAttribute, 1, _loginWithFudist.Frame.Width));
            View.AddConstraint(NSLayoutConstraint.Create(_loginWithFudist, NSLayoutAttribute.Height, NSLayoutRelation.Equal,
                null, NSLayoutAttribute.NoAttribute, 1, _loginWithFudist.Frame.Height));
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            MessageLabel.TextColor = FudistColors.WarningColor;
        }

        private void LoginWithFudistOnTouchUpInside(object sender, EventArgs eventArgs)
        {
            PerformSegue("GoToFudistLogin", this);
        }

        partial void GoogleButton_TouchUpInside(UIButton sender)
        {
            ClearMessageText();
            LoginToGoogle(true);
        }
        
        partial void FacebookButton_TouchUpInside(UIButton sender)
        {
            ClearMessageText();
            LoginToFacebook(true);
        }

        void LoginToGoogle(bool allowCancel)
        {
            var auth = new OAuth2Authenticator(
                clientId: "700800275641-m2kon3vbvb0em6bu35qjdqbqnd2qilib.apps.googleusercontent.com",
                scope: "https://www.googleapis.com/auth/userinfo.email",
                authorizeUrl: new Uri("https://accounts.google.com/o/oauth2/auth"),
                redirectUrl: new Uri("https://api.fudist.co/oauth2callback"))
            { AllowCancel = allowCancel };


            auth.Completed += (sender, e) =>
            {
                SocialLoginComplete(e, "google");
            };

            var vc = auth.GetUI();
            PresentViewController(vc, true, null);

        }

        void LoginToFacebook(bool allowCancel)
        {
            var auth = new OAuth2Authenticator(
                clientId: "671043732908389",
                scope: "public_profile, email",
                authorizeUrl: new Uri("https://m.facebook.com/dialog/oauth/"),
                redirectUrl: new Uri("https://api.fudist.co/api/ExternalLogin"))
            { AllowCancel = allowCancel };


            // If authorization succeeds or is canceled, .Completed will be fired.
            auth.Completed += (s, e) => {
                SocialLoginComplete(e, "facebook");
            };

            var vc = auth.GetUI();
            PresentViewController(vc, true, null);

        }

        async void SocialLoginComplete(AuthenticatorCompletedEventArgs e, string socialProvider)
        {
            if (!e.IsAuthenticated)
            {
                DismissViewController(true, null);
                return;
            }
            string accessToken;
            e.Account.Properties.TryGetValue("access_token", out accessToken);
            Person person;
            try
            {
                _loadingOverlay = new LoadingOverlay(UIScreen.MainScreen.Bounds, "Signing In...");
                View.Add(_loadingOverlay);
                person = await FudistPersonAdapter.AuthPersonWithToken(accessToken, socialProvider, UnifiedAnalytics.GetInstance(), AuthorizationSingleton.GetInstance());
                Data.SignUserIn(person, AuthorizationSingleton.PersonIsProOrHigherUser(person));
            }
            catch
            {
                DismissViewController(true, () =>
                {
                    SetMessageText("Sign in failure. Please check that you have a Fudist subscription");
                    _loadingOverlay.Hide();
                });
                return;
            }
            var authorizationSingleton = AuthorizationSingleton.GetInstance(UnifiedAnalytics.GetInstance());

            authorizationSingleton.SetPerson(person);

            //First, dismiss the social login VC. When that is done, ask our parent view controller to dismiss us.
            DismissViewController(true, () => DismissViewController(true, () => { }));
        }

        private void SetMessageText(string text)
        {
            MessageLabel.Layer.Frame = new CGRect(
                MessageLabel.Layer.Frame.X,
                MessageLabel.Layer.Frame.Y,
                MessageLabel.Layer.Frame.Width,
                41);
            MessageLabel.Text = text;
        }

        private void ClearMessageText()
        {
            MessageLabel.Layer.Frame = new CGRect(
                MessageLabel.Layer.Frame.X,
                MessageLabel.Layer.Frame.Y,
                MessageLabel.Layer.Frame.Width,
                0);
            MessageLabel.Text = string.Empty;
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            BackButton.Clicked += BackButton_Clicked;
        }

        private void BackButton_Clicked(object sender, EventArgs e)
        {
            DismissViewController(true, () => { });
        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
            BackButton.Clicked -= BackButton_Clicked;
        }
    }
}