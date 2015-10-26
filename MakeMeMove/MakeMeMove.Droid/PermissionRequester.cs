using System.IO;
using Android.App;
using Android.OS;
using MakeMeMove.Droid;
using Newtonsoft.Json;
using Xamarin.Forms;
using Environment = System.Environment;

[assembly: Dependency(typeof(PermissionRequester))]
namespace MakeMeMove.Droid
{
    public class PermissionRequester : IPermissionRequester
    {
        public void RequestPermissions()
        {

            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            var filePath = Path.Combine(documentsPath, Constants.NotificationPreferences);



            if ((int)Build.VERSION.SdkInt >= 23 && !File.Exists(filePath))
            {
                new AlertDialog.Builder(Forms.Context)
                    .SetMessage(
                        "Will you allow Make Me Move to create high priority notifications that will wake your device (could affect battery life)? Without these, your reminders may not appear if your phone is sitting on your desk.")
                    .SetNegativeButton("No", (s, args) =>
                    {
                        File.WriteAllText(filePath, JsonConvert.SerializeObject(false));
                    })
                    .SetPositiveButton("Yes", (s, args) =>
                    {
                        File.WriteAllText(filePath, JsonConvert.SerializeObject(true));
                    })
                    .SetCancelable(false)
                    .SetTitle("Notification Permissions").Create().Show();
            }
        }
    }
}