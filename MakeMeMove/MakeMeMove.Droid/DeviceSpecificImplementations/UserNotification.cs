using System;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using MakeMeMove.Droid.Activities;
using MakeMeMove.Droid.ExerciseActions;
using MakeMeMove.Droid.Utilities;

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
            var userIsPremium = data.UserIsPremium();
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

            PendingIntent clickPendingIntent;
            if (userIsPremium)
            {
                var clickIntent = new Intent(context, typeof (ExerciseHistoryActivity));
                clickIntent.PutExtra(Constants.ShowMarkedExercisePrompt, true);
                clickIntent.PutExtra(Constants.ExerciseId, nextExercise.Id);
                var stackBuilder = TaskStackBuilder.Create(context);
                stackBuilder.AddParentStack(Java.Lang.Class.FromType(typeof (ExerciseHistoryActivity)));
                stackBuilder.AddNextIntent(clickIntent);
                clickPendingIntent = stackBuilder.GetPendingIntent(0, PendingIntentFlags.CancelCurrent);
            }
            else
            {
                var clickIntent = new Intent(context, typeof(MainActivity));
                clickPendingIntent = PendingIntent.GetActivity(context, DateTime.Now.Millisecond, clickIntent, PendingIntentFlags.CancelCurrent);
            }

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
                    .AddAction(new Notification.Action(Resource.Drawable.ic_shuffle_black_24dp, "Change", nextPendingIntent));
                if (userIsPremium)
                {
                    builder
                        .AddAction(new Notification.Action(Resource.Drawable.ic_done_black_24dp, "Completed", completedPendingIntent));
                }
            }
            else if ((int)Build.VERSION.SdkInt >= 20)
            {
                builder
                    .SetSmallIcon(Resource.Drawable.icon)
                    .AddAction(new Notification.Action(Resource.Drawable.ic_shuffle_black_24dp, "Change", nextPendingIntent));
                if (userIsPremium)
                {
                    builder
                        .AddAction(new Notification.Action(Resource.Drawable.ic_done_black_24dp, "Completed", completedPendingIntent));
                }
            }
            else
            {
                // Yes, resharper, I know this is deprecated. but this is how you did it in pre-20
                builder
                    .SetSmallIcon(Resource.Drawable.icon)
                    .AddAction(Resource.Drawable.ic_shuffle_white_24dp, "Change", nextPendingIntent);
                if (userIsPremium)
                {
                    builder
                        .AddAction(Resource.Drawable.ic_done_white_24dp, "Completed", completedPendingIntent);
                }
            }

            var notification = builder.Build();

            notification.Flags |= NotificationFlags.AutoCancel;

            var notificationManager = context.GetSystemService(Context.NotificationService) as NotificationManager;
            notificationManager?.Notify(Constants.NotificationId, notification);
        }
    }
}