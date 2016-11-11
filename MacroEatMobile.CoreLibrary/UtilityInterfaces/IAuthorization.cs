using MacroEatMobile.Core;
using System.Threading.Tasks;

namespace MacroEatMobile.iPhone.Utilities
{
    public interface IAuthorization
    {
        Task<Person> GetPerson();
        Task RefreshPerson();
        void SetPerson(Person person);
        AuthToken GetAuthToken();
        void SetAuthToken(AuthToken authToken);
        void ClearPerson();
        Task<bool> CurrentPersonIsUserAdmin();
    }
}