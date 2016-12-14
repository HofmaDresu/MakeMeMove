using System;
using System.Linq;
using System.Threading.Tasks;
using Foundation;
using MacroEatMobile.Core;
using MacroEatMobile.Core.UtilityInterfaces;
using MacroEatMobile.iPhone.Utilities;

namespace MakeMeMove.iOS.Utilities
{
    public class AuthorizationSingleton : IAuthorization
    {
        readonly IUnifiedAnalytics _unifiedAnalytics;
        public EventHandler UserLoggedOut;
        private AuthorizationSingleton(IUnifiedAnalytics unifiedAnalytics)
        {
            _unifiedAnalytics = unifiedAnalytics;
        }

        private AuthToken _authToken;
        private Person _person;

        public static Action<AuthToken> TokenPersister = token => {
            GetInstance().SetAuthToken(token);
        };

        public Person GetPersonSync()
        {
            var t = GetPerson();
            t.ConfigureAwait(false);

            return t.Result;
        }

        public async Task<Person> GetPerson()
        {
            if (_person != null) return _person;

            var authToken = GetAuthToken();

            if (authToken == null)
                return null;

            _person = await FudistPersonAdapter.GetAuthenticatedPerson(GetInstance(), _unifiedAnalytics);

            return _person;
        }

        public async Task RefreshPerson()
        {
            if (_person == null || !_person.IsGuestUser) _person = null;
            await GetPerson();
        }

        public void SetPerson(Person person)
        {
            _person = person;
        }

        public AuthToken GetAuthToken()
        {

            if (_authToken != null)
                return _authToken;

            var standardUserDefaults = NSUserDefaults.StandardUserDefaults;
            var token = standardUserDefaults.StringForKey("authToken");
            var exp = standardUserDefaults.StringForKey("authTokenExp");
            var refresh = standardUserDefaults.StringForKey("refreshToken");

            if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(exp))
            {
                return null;
            }

            var tokenExp = DateTime.FromBinary(long.Parse(exp));

            var authToken = new AuthToken
            {
                AccessToken = token,
                ExpirationDate = tokenExp,
                RefreshToken = refresh
            };

            return authToken;
        }

        public void SetAuthToken(AuthToken authToken)
        {
            _authToken = authToken;
            SaveToken(authToken);
        }

        private void SaveToken(AuthToken authToken)
        {
            var standardUserDefaults = NSUserDefaults.StandardUserDefaults;

            var expAsString = authToken.ExpirationDate?.ToBinary().ToString() ?? "";

            standardUserDefaults.SetString(authToken.AccessToken ?? "", "authToken");
            standardUserDefaults.SetString(expAsString, "authTokenExp");
            standardUserDefaults.SetString(authToken.RefreshToken ?? "", "refreshToken");

            Console.WriteLine("saved token to prefs {0}", authToken.AccessToken);
        }

        public void ClearPerson()
        {
            _person = null;
            SaveToken(new AuthToken());
            UserLoggedOut?.Invoke(this, new EventArgs());
        }

        public void RevokeAuthTokenOnly()
        {
            var currentAuthToken = GetAuthToken();

            SetAuthToken(new AuthToken
            {
                AccessToken = currentAuthToken.AccessToken,
                ExpirationDate = DateTime.UtcNow.AddDays(-1),
                RefreshToken = currentAuthToken.RefreshToken
            });
        }

        public async Task<bool> CurrentPersonIsUserAdmin()
        {
            var person = await GetPerson();
            return person?.Roles != null && person.Roles.Any(r => r.Token == "UserAdmin");
        }

        public static bool PersonIsProOrHigherUser(Person person)
        {
            return person?.Roles != null && person.Roles.Any(r => r.Token == "ProUser");
        }

        private static AuthorizationSingleton _authSing;
        public static AuthorizationSingleton GetInstance(IUnifiedAnalytics analytics)
        {
            return _authSing ?? (_authSing = new AuthorizationSingleton(analytics));
        }

        public static AuthorizationSingleton GetInstance()
        {
            return _authSing ?? (_authSing = new AuthorizationSingleton(UnifiedAnalytics.GetInstance()));
        }

    }
}
