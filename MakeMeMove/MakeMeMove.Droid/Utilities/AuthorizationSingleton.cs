using System;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using MacroEatMobile.Core;
using MacroEatMobile.iPhone.Utilities;

namespace MakeMeMove.Droid.Utilities
{
    public class AuthorizationSingleton
    {
        private AuthorizationSingleton()
        {
        }

        private Person _person;
        private AuthToken _authToken;

        public Person GetPersonSync(Activity context, bool forceRefresh = false)
        {
            var t = GetPerson(context, forceRefresh);
            t.ConfigureAwait(false);

            return t.Result;
        }

        public async Task<Person> GetPerson(Activity context, bool forceRefresh = false)
        {
            if (_person != null && (!forceRefresh || _person.IsGuestUser)) return _person;

            var authToken = GetAuthToken(context);

            if (authToken == null)
                return null;

            _person = await FudistPersonAdapter.GetAuthenticatedPerson(CreateIAuthorization(context), UnifiedAnalytics.GetInstance(context));

            return _person;
        }

        public void SetPerson(Person person, Context context)
        {
            _person = person;
        }

        public AuthToken GetAuthToken(Context context)
        {

            if (_authToken != null)
            {
                return _authToken;
            }


            var prefs = context.GetSharedPreferences("fudistConfig", FileCreationMode.Private);
            var token = prefs.GetString("authToken", "");
            var exp = prefs.GetString("authTokenExp", "");
            var refresh = prefs.GetString("refreshToken", "");

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

        public void SetAuthToken(AuthToken authToken, Context context)
        {
            _authToken = authToken;
            SaveToken(authToken, context);
        }

        public void SaveToken(AuthToken authToken, Context context)
        {
            var prefs = context.GetSharedPreferences("fudistConfig", FileCreationMode.Private);
            var editor = prefs.Edit();

            var expAsString = authToken.ExpirationDate?.ToBinary().ToString() ?? "";

            editor.PutString("authToken", authToken.AccessToken);
            editor.PutString("authTokenExp", expAsString);
            editor.PutString("refreshToken", authToken.RefreshToken);

            editor.Commit();

            Console.WriteLine("saved token to prefs {0}", authToken.AccessToken);
        }

        public void RemoveSavedToken(Context context)
        {
            var prefs = context.GetSharedPreferences("fudistConfig", FileCreationMode.Private);
            var editor = prefs.Edit();

            editor.Remove("authToken");
            editor.Remove("authTokenExp");
            editor.Remove("refreshToken");

            editor.Commit();

            _authToken = null;
        }

        public void RevokeAuthTokenOnly(Context context)
        {
            var prefs = context.GetSharedPreferences("fudistConfig", FileCreationMode.Private);
            var editor = prefs.Edit();


            editor.PutString("authTokenExp", DateTime.UtcNow.AddDays(-1).ToBinary().ToString());
            _authToken = null;

            editor.Commit();
        }

        public void ClearPerson(Context context)
        {
            _person = null;

            RemoveSavedToken(context);
        }

        public async Task<bool> CurrentPersonIsUserAdmin(Activity context)
        {
            var person = await GetPerson(context);
            return person?.Roles != null && person.Roles.Any(r => r.Token == "UserAdmin");
        }

        public static bool PersonIsProOrHigherUser(Person person)
        {
            return person?.Roles != null && person.Roles.Any(r => r.Token == "ProUser");
        }

        private static AuthorizationSingleton _authSing;
        public static AuthorizationSingleton GetInstance()
        {
            return _authSing ?? (_authSing = new AuthorizationSingleton());
        }


        public static ConcreteAuthorization CreateIAuthorization(Activity context)
        {
            var authSing = GetInstance();

            return new ConcreteAuthorization
            {
                OnClearPerson = () => authSing.ClearPerson(context),
                OnCurrentPersonIsUserAdmin = () => authSing.CurrentPersonIsUserAdmin(context),
                OnRefreshPerson = async () => { await authSing.GetPerson(context, true); },
                OnGetAuthToken = () => authSing.GetAuthToken(context),
                OnSetAuthToken = authToken => authSing.SetAuthToken(authToken, context),
                OnGetPerson = () => authSing.GetPerson(context),
                OnSetPerson = person => authSing.SetPerson(person, context)
            };
        }

        public class ConcreteAuthorization : IAuthorization
        {
            public Func<Task<Person>> OnGetPerson;
            public Func<Task> OnRefreshPerson;
            public Action<Person> OnSetPerson;
            public Func<AuthToken> OnGetAuthToken;
            public Action<AuthToken> OnSetAuthToken;
            public Action OnClearPerson;
            public Func<Task<bool>> OnCurrentPersonIsUserAdmin;

            public Task<Person> GetPerson()
            {
                return OnGetPerson();
            }

            public Task RefreshPerson()
            {
                return OnRefreshPerson();
            }

            public void SetPerson(Person person)
            {
                OnSetPerson(person);
            }

            public AuthToken GetAuthToken()
            {
                return OnGetAuthToken();
            }

            public void SetAuthToken(AuthToken authToken)
            {
                OnSetAuthToken(authToken);
            }

            public void ClearPerson()
            {
                OnClearPerson();
            }

            public Task<bool> CurrentPersonIsUserAdmin()
            {
                return OnCurrentPersonIsUserAdmin();
            }

        }

    }
}

