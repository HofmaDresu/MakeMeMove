using System;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Support.V4.App;
using MakeMeMove.Droid.Activities;
using MakeMeMove.Droid.BroadcastReceivers;
using MakeMeMove.Model;
using TaskStackBuilder = Android.App.TaskStackBuilder;
using Android.Preferences;
using Android.Media;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace MakeMeMove.Droid.DeviceSpecificImplementations
{
    public class UserNotification
    {
        public void ShowValidationErrorPopUp(Context context, int messageId, Action onCloseAction = null)
        {
            new AlertDialog.Builder(context)
                .SetTitle(Resource.String.ValidationTitle)
                .SetMessage(messageId)
                .SetCancelable(false)
                .SetPositiveButton(Resource.String.Ok, (sender, args) => { onCloseAction?.Invoke();})
                .Show();
        }

        public void ShowAreYouSureDialog(Context context, string message, Action onYesAction = null, Action onNoAction = null)
        {
            new AlertDialog.Builder(context)
                .SetTitle(Resource.String.AreYouSureTitle)
                .SetMessage(message)
                .SetCancelable(false)
                .SetPositiveButton(Resource.String.Yes, (sender, args) => { onYesAction?.Invoke(); })
                .SetNegativeButton(Resource.String.No, (sender, args) => { onNoAction?.Invoke(); })
                .Show();
        }

        public static async Task CreateExerciseNotification(Data data, Context context)
        {
            var showExerciseNotificaiton = false;

            try
            {
                var isMovementLocationsEnabled = data.IsMovementLocationsEnabled();
                var movementLocations = data.GetMovementLocations();

                if (isMovementLocationsEnabled && movementLocations.Any())
                {
                    var lastKnownLocation = await Geolocation.GetLastKnownLocationAsync();
                    var inMovementLocation = movementLocations.Any(ml => Location.CalculateDistance(lastKnownLocation, new Location(ml.Latitude, ml.Longitude), DistanceUnits.Miles) < .2);

                    showExerciseNotificaiton = inMovementLocation;
                }
                else
                {
                    showExerciseNotificaiton = true;
                }
            }
            catch (Exception)
            {
                showExerciseNotificaiton = true;
            }

            if (showExerciseNotificaiton)
            {
                var nextExercise = data.GetNextEnabledExercise();
                CreateExerciseNotification(data, context, nextExercise);

            }
        }

        public static void CreateExerciseNotification(Data data, Context context, ExerciseBlock nextExercise)
        {
            if (nextExercise == null) return;

            var completedIntent = new Intent(context, typeof(CompleteExerciseBroadcastReceiver));
            completedIntent.PutExtra(Constants.ExerciseName, nextExercise.CombinedName);
            completedIntent.PutExtra(Constants.ExerciseQuantity, nextExercise.Quantity);
            var completedPendingIntent = PendingIntent.GetBroadcast(context, DateTime.Now.Millisecond, completedIntent, PendingIntentFlags.OneShot);
            
            var nextIntent = new Intent(context, typeof(ChangeExerciseBroadcastReceiver));
            nextIntent.PutExtra(Constants.ExerciseName, nextExercise.CombinedName);
            nextIntent.PutExtra(Constants.ExerciseQuantity, nextExercise.Quantity);
            var nextPendingIntent = PendingIntent.GetBroadcast(context, DateTime.Now.Millisecond, nextIntent, PendingIntentFlags.CancelCurrent);


            var clickIntent = new Intent(context, typeof (ExerciseHistoryActivity));
            clickIntent.PutExtra(Constants.ShowMarkedExercisePrompt, true);
            clickIntent.PutExtra(Constants.ExerciseId, nextExercise.Id);
            var stackBuilder = TaskStackBuilder.Create(context);
            stackBuilder.AddParentStack(Java.Lang.Class.FromType(typeof (ExerciseHistoryActivity)));
            stackBuilder.AddNextIntent(clickIntent);
            var clickPendingIntent = stackBuilder.GetPendingIntent(0, PendingIntentFlags.CancelCurrent);

            data.MarkExerciseNotified(nextExercise.CombinedName, nextExercise.Quantity);

            var timeToMoveMessage = string.Format(context.Resources.GetString(Resource.String.TimeToMoveMessage), nextExercise.Quantity, nextExercise.CombinedName);
            var builder = GetBuilder(context, Constants.ExerciseNotificationChannelId)
                .SetContentTitle(context.Resources.GetString(Resource.String.TimeToMoveTitle))
                .SetContentText(timeToMoveMessage)
                .SetDefaults(NotificationCompat.DefaultVibrate)
                .SetContentIntent(clickPendingIntent)
                .SetSound(Android.Net.Uri.Parse(PreferenceManager.GetDefaultSharedPreferences(context).GetString(context.Resources.GetString(Resource.String.NotificationSoundKey), RingtoneManager.GetDefaultUri(RingtoneType.Notification).ToString())));


            var changeExerciseButtonText = context.Resources.GetString(Resource.String.ChangeExerciseButtonText);
            var completedButtonText = context.Resources.GetString(Resource.String.CompletedButtonText);

            builder
                .SetPriority((int) NotificationPriority.High)
                .SetVisibility(NotificationCompat.VisibilityPublic)
                .SetCategory("reminder")
                .SetSmallIcon(Resource.Drawable.Mmm_white_icon)
                .SetColor(Color.Rgb(215, 78, 10))
                .AddAction(new NotificationCompat.Action(Resource.Drawable.ic_shuffle_black_24dp, changeExerciseButtonText, nextPendingIntent))
                .AddAction(new NotificationCompat.Action(Resource.Drawable.ic_done_black_24dp, completedButtonText, completedPendingIntent));

            var notification = builder.Build();

            notification.Flags |= NotificationFlags.AutoCancel;

            var notificationManager = NotificationManagerCompat.From(context);
            notificationManager?.Notify(Constants.ExerciseNotificationId, notification);
        }

        public static void CreateHistoryReminderNotification(Context context)
        {
            var clickIntent = new Intent(context, typeof(ExerciseHistoryActivity));
            clickIntent.PutExtra(Constants.ShowMarkedExercisePrompt, false);
            var stackBuilder = TaskStackBuilder.Create(context);
            stackBuilder.AddParentStack(Java.Lang.Class.FromType(typeof(ExerciseHistoryActivity)));
            stackBuilder.AddNextIntent(clickIntent);
            var clickPendingIntent = stackBuilder.GetPendingIntent(0, PendingIntentFlags.CancelCurrent);


            var builder = GetBuilder(context, Constants.TodaysProgressNotificationChannelId)
                .SetContentTitle(context.Resources.GetString(Resource.String.CheckHistoryNotificationTitle))
                .SetContentText(context.Resources.GetString(Resource.String.CheckHistoryNotificationMessage))
                .SetDefaults(NotificationCompat.DefaultVibrate)
                .SetContentIntent(clickPendingIntent)
                .SetSound(Android.Net.Uri.Parse(PreferenceManager.GetDefaultSharedPreferences(context).GetString(context.Resources.GetString(Resource.String.NotificationSoundKey), RingtoneManager.GetDefaultUri(RingtoneType.Notification).ToString())))
                .SetPriority((int)NotificationPriority.High)
                .SetVisibility(NotificationCompat.VisibilityPublic)
                .SetCategory("reminder")
                .SetSmallIcon(Resource.Drawable.Mmm_white_icon)
                .SetColor(Color.Rgb(215, 78, 10));

            var notification = builder.Build();

            notification.Flags |= NotificationFlags.AutoCancel;

            var notificationManager = NotificationManagerCompat.From(context);
            notificationManager?.Notify(Constants.HistoryReminderNotificationId, notification);
        }

        private static NotificationCompat.Builder GetBuilder(Context context, string channelId)
        {
            return Build.VERSION.SdkInt >= BuildVersionCodes.O ? new NotificationCompat.Builder(context, channelId) : new NotificationCompat.Builder(context);
        }
    }
}