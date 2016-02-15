using System;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using MakeMeMove.Droid.Activities;
using MakeMeMove.Droid.ExerciseActions;

namespace MakeMeMove.Droid.DeviceSpecificImplementations
{
    public class UserNotification
    {
        public void ShowValidationErrorPopUp(Context context, string message, Action onCloseAction = null)
        {
            new AlertDialog.Builder(context)
                .SetTitle("Invalid Information")
                .SetMessage(message)
                .SetCancelable(false)
                .SetPositiveButton("OK", (sender, args) => { onCloseAction?.Invoke();})
                .Show();
        }

        public void ShowAreYouSureDialog(Context context, string message, Action onYesAction = null, Action onNoAction = null)
        {
            new AlertDialog.Builder(context)
                .SetTitle("Are you sure?")
                .SetMessage(message)
                .SetCancelable(false)
                .SetPositiveButton("Yes", (sender, args) => { onYesAction?.Invoke(); })
                .SetNegativeButton("No", (sender, args) => { onNoAction?.Invoke(); })
                .Show();
        }

        public static void CreateNotification(Data data, Context context)
        {
            var nextExercise = data.GetNextEnabledExercise();
            if (nextExercise == null) return;

            var completedIntent = new Intent(context, typeof(CompletedActivity));
            completedIntent.PutExtra(Constants.ExerciseName, nextExercise.CombinedName);
            completedIntent.PutExtra(Constants.ExerciseQuantity, nextExercise.Quantity);
            var completedPendingIntent = PendingIntent.GetActivity(context, DateTime.Now.Millisecond, completedIntent, PendingIntentFlags.CancelCurrent);

            var nextIntent = new Intent(context, typeof(NextActivity));
            nextIntent.PutExtra(Constants.ExerciseName, nextExercise.CombinedName);
            nextIntent.PutExtra(Constants.ExerciseQuantity, nextExercise.Quantity);
            var nextPendingIntent = PendingIntent.GetActivity(context, DateTime.Now.Millisecond, nextIntent, PendingIntentFlags.CancelCurrent);

            
            var clickIntent = new Intent(context, typeof(ExerciseHistoryActivity));
            clickIntent.PutExtra(Constants.ShowMarkedExercisePrompt, true);
            clickIntent.PutExtra(Constants.ExerciseId, nextExercise.Id);
            var stackBuilder = TaskStackBuilder.Create(context);
            stackBuilder.AddParentStack(Java.Lang.Class.FromType(typeof(ExerciseHistoryActivity)));
            stackBuilder.AddNextIntent(clickIntent);
            var clickPendingIntent = stackBuilder.GetPendingIntent(0, PendingIntentFlags.CancelCurrent);

            data.MarkExerciseNotified(nextExercise.CombinedName, nextExercise.Quantity);

            var builder = new Notification.Builder(context)
                .SetContentTitle("Time to Move")
                .SetContentText($"It's time to do {nextExercise.Quantity} {nextExercise.CombinedName}")
                .SetDefaults(NotificationDefaults.Sound | NotificationDefaults.Vibrate)
                .SetContentIntent(clickPendingIntent);

            if ((int)Build.VERSION.SdkInt >= 21)
            {
                builder
                    .SetPriority((int)NotificationPriority.High)
                    .SetVisibility(NotificationVisibility.Public)
                    .SetCategory("reminder")
                    .SetSmallIcon(Resource.Drawable.Mmm_white_icon)
                    .SetColor(Color.Rgb(215, 78, 10))
                    .AddAction(new Notification.Action(0, "Completed", completedPendingIntent))
                    .AddAction(new Notification.Action(0, "Next", nextPendingIntent));
            }
            else if ((int)Build.VERSION.SdkInt >= 20)
            {
                builder
                    .SetSmallIcon(Resource.Drawable.icon)
                    .AddAction(new Notification.Action(0, "Completed", completedPendingIntent))
                    .AddAction(new Notification.Action(0, "Next", nextPendingIntent));
            }
            else
            {
                builder
                    .SetSmallIcon(Resource.Drawable.icon)
                    .AddAction(0, "Completed", completedPendingIntent)
                    .AddAction(0, "Next", nextPendingIntent);
            }

            var notification = builder.Build();

            notification.Flags |= NotificationFlags.AutoCancel;

            var notificationManager = context.GetSystemService(Context.NotificationService) as NotificationManager;
            notificationManager?.Notify(Constants.NotificationId, notification);
        }
    }
}