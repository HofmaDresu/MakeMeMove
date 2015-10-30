using MakeMeMove.DeviceSpecificInterfaces;
using MakeMeMove.iOS.DeviceSpecificImplementations;
using Xamarin.Forms;

[assembly: Dependency(typeof(PermissionRequester))]
namespace MakeMeMove.iOS.DeviceSpecificImplementations
{
    public class PermissionRequester : IPermissionRequester
    {
        public void RequestPermissions()
        {
            
        }
    }
}