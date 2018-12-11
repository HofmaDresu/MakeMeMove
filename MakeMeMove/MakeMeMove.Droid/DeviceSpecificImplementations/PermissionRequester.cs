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

            if (!File.Exists(filePath))
            {
                new AlertDialog.Builder(context)
                    .SetMessage(Resource.String.PermissionRequesterMessage)
                    .SetNegativeButton(Resource.String.No, (s, args) =>
                    {
                        File.WriteAllText(filePath, JsonConvert.SerializeObject(false));
                    })
                    .SetPositiveButton(Resource.String.Yes, (s, args) =>
                    {
                        File.WriteAllText(filePath, JsonConvert.SerializeObject(true));
                    })
                    .SetCancelable(false)
                    .SetTitle(Resource.String.PermissionRequesterTitle).Create().Show();
            }
        }
    }
}