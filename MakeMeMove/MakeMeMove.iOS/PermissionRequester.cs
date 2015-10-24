using MakeMeMove.iOS;
using Xamarin.Forms;

[assembly: Dependency(typeof(PermissionRequester))]
namespace MakeMeMove.iOS
{
    public class PermissionRequester : IPermissionRequester
    {
        public void RequestPermissions()
        {
            
        }
    }
}