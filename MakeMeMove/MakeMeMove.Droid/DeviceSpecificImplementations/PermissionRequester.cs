using System.IO;
using Android.App;
using Android.Content;
using Android.OS;
using Newtonsoft.Json;
using Environment = System.Environment;

namespace MakeMeMove.Droid.DeviceSpecificImplementations
{
    public class PermissionRequester
    {
        public void RequestPermissions(Context context)
        {

            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            var filePath = Path.Combine(documentsPath, Constants.NotificationPreferences);



            if (Build.VERSION.SdkInt >= BuildVersionCodes.M && !File.Exists(filePath))
            {
                new AlertDialog.Builder(context)
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