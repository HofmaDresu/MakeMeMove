using Foundation;
using System;
using System.Threading.Tasks;
using CoreGraphics;
using MacroEatMobile.Core;
using MakeMeMove.iOS.Controls;
using MakeMeMove.iOS.Helpers;
using MakeMeMove.iOS.Utilities;
using MakeMeMove.iOS.ViewControllers.Base;
using UIKit;

namespace MakeMeMove.iOS
{
    public partial class FudistLoginViewController : BaseViewController
    {
        LoadingOverlay _loadingOverlay;

        public FudistLoginViewController (IntPtr handle) : base (handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            NavigationController.NavigationBar.TintColor = UIColor.White;
            Message.Text = "";

            Message.Layer.Frame = new CGRect(
                Message.Layer.Frame.X,
                Message.Layer.Frame.Y,
                Message.Layer.Frame.Width,
                0);

            UserName.ShouldReturn += textField =>
            {
                textField.ResignFirstResponder();
                Password.BecomeFirstResponder();
                return true;
            };
            Password.ShouldReturn += textField =>
            {
                textField.ResignFirstResponder();
                AttemptLogin();
                return true;
            };

            SetupLoginButton();
            SetupCancelButton();
        }

        private void SetupCancelButton()
        {
            var cancelButton = new FloatingButton("Cancel");
            cancelButton.TouchUpInside += (sender, e) => PerformSegue("GoToLoginChoice", this);
            cancelButton.TranslatesAutoresizingMaskIntoConstraints = false;
            View.Add(cancelButton);

            View.AddConstraint(NSLayoutConstraint.Create(cancelButton, NSLayoutAttribute.Top, NSLayoutRelation.Equal,
                Password, NSLayoutAttribute.Bottom, 1, 20));
            View.AddConstraint(NSLayoutConstraint.Create(cancelButton, NSLayoutAttribute.Left, NSLayoutRelation.Equal,
                Password, NSLayoutAttribute.Left, 1, 0));
            View.AddConstraint(NSLayoutConstraint.Create(cancelButton, NSLayoutAttribute.Width, NSLayoutRelation.Equal,
                null, NSLayoutAttribute.NoAttribute, 1, cancelButton.Frame.Width));
            View.AddConstraint(NSLayoutConstraint.Create(cancelButton, NSLayoutAttribute.Height, NSLayoutRelation.Equal,
                null, NSLayoutAttribute.NoAttribute, 1, cancelButton.Frame.Height));
        }

        private void SetupLoginButton()
        {
            var loginButton = new FloatingButton("Sign In");
            loginButton.TouchUpInside += LoginButtonTouchUpInside;
            loginButton.TranslatesAutoresizingMaskIntoConstraints = false;
            View.Add(loginButton);

            View.AddConstraint(NSLayoutConstraint.Create(loginButton, NSLayoutAttribute.Top, NSLayoutRelation.Equal,
                Password, NSLayoutAttribute.Bottom, 1, 20));
            View.AddConstraint(NSLayoutConstraint.Create(loginButton, NSLayoutAttribute.Right, NSLayoutRelation.Equal,
                Password, NSLayoutAttribute.Right, 1, 0));
            View.AddConstraint(NSLayoutConstraint.Create(loginButton, NSLayoutAttribute.Width, NSLayoutRelation.Equal,
                null, NSLayoutAttribute.NoAttribute, 1, loginButton.Frame.Width));
            View.AddConstraint(NSLayoutConstraint.Create(loginButton, NSLayoutAttribute.Height, NSLayoutRelation.Equal,
                null, NSLayoutAttribute.NoAttribute, 1, loginButton.Frame.Height));
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            Message.TextColor = FudistColors.WarningColor;
            Message.Frame = new CGRect(
                Message.Frame.X,
                Message.Frame.Y,
                Message.Frame.Width,
                0);
        }

        void LoginButtonTouchUpInside(object sender, EventArgs e)
        {
            AttemptLogin();
        }

        private void AttemptLogin()
        {
            UserName.ResignFirstResponder();
            Password.ResignFirstResponder();
            _loadingOverlay = new LoadingOverlay(UIScreen.MainScreen.Bounds, "Signing In...");
            View.Add(_loadingOverlay);

            var analytics = UnifiedAnalytics.GetInstance();
            var authorizationSingleton = AuthorizationSingleton.GetInstance();
            var username = UserName.Text;
            var password = Password.Text;

            FudistPersonAdapter.AuthPerson(username, password, analytics, authorizationSingleton)
                .ContinueWith(t => InvokeOnMainThread(() =>
                {
                    _loadingOverlay.Hide();
                    var person = t.Result;
                    if (person == null)
                    {
                        Password.Text = "";
                        SetMessageText("Username or password was incorrect. Please try again.");
                        return;
                    }
                    authorizationSingleton.SetPerson(person);

                    Data.SignUserIn(person, AuthorizationSingleton.PersonIsProOrHigherUser(person));
                    DismissViewController(true, () => DismissViewController(true, () => { }));
                }), TaskContinuationOptions.OnlyOnRanToCompletion)
                .ContinueWith(t => InvokeOnMainThread(() =>
                {
                    _loadingOverlay.Hide();
                    Password.Text = "";
                    SetMessageText("There was an error signing in. Please try again.");
                }), TaskContinuationOptions.NotOnRanToCompletion);
        }

        private void SetMessageText(string text)
        {
            Message.Layer.Frame = new CGRect(
                Message.Layer.Frame.X,
                Message.Layer.Frame.Y,
                Message.Layer.Frame.Width,
                41);
            Message.Text = text;
            _loadingOverlay.Hide();
        }
    }
}