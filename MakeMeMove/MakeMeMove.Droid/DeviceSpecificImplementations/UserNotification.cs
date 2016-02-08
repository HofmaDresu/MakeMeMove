using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using MakeMeMove.Droid.ExerciseActions;
using MakeMeMove.Model;
using static System.Math;

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
            var exercises = data.GetExerciseBlocks();
            var enabledExercises = exercises.Where(e => e.Enabled).ToList();

            if (enabledExercises.Count == 0) return;

            var index = new Random().Next(0, enabledExercises.Count);

            var nextExercise = enabledExercises[Min(index, exercises.Count - 1)];

            var completedIntent = new Intent(context, typeof(CompletedActivity));
            completedIntent.PutExtra(Constants.ExerciseName, nextExercise.CombinedName);
            completedIntent.PutExtra(Constants.ExerciseQuantity, nextExercise.Quantity);
            var completedPendingIntent = PendingIntent.GetActivity(context, DateTime.Now.Millisecond, completedIntent, PendingIntentFlags.CancelCurrent);

            var nextIntent = new Intent(context, typeof(NextActivity));
            nextIntent.PutExtra(Constants.ExerciseName, nextExercise.CombinedName);
            nextIntent.PutExtra(Constants.ExerciseQuantity, nextExercise.Quantity);
            var nextPendingIntent = PendingIntent.GetActivity(context, DateTime.Now.Millisecond, nextIntent, PendingIntentFlags.CancelCurrent);

            data.MarkExerciseNotified(nextExercise.CombinedName, nextExercise.Quantity);

            var builder = new Notification.Builder(context)
                .SetContentTitle("Time to Move")
                .SetContentText($"It's time to do {nextExercise.Quantity} {nextExercise.CombinedName}")
                .SetDefaults(NotificationDefaults.Sound | NotificationDefaults.Vibrate)
                .AddAction(new Notification.Action(0, "Completed", completedPendingIntent))
                .AddAction(new Notification.Action(0, "Next", nextPendingIntent));

            if ((int)Build.VERSION.SdkInt >= 21)
            {
                builder
                    .SetPriority((int)NotificationPriority.High)
                    .SetVisibility(NotificationVisibility.Public)
                    .SetCategory("reminder")
                    .SetSmallIcon(Resource.Drawable.Mmm_white_icon)
                    .SetColor(Color.Rgb(215, 78, 10));
            }
            else if ((int)Build.VERSION.SdkInt >= 20)
            {
                builder
                    .SetSmallIcon(Resource.Drawable.icon);
            }
            else
            {
                builder
                    .SetSmallIcon(Resource.Drawable.icon);
            }

            var notification = builder.Build();

            notification.Flags |= NotificationFlags.AutoCancel;

            var notificationManager = context.GetSystemService(Context.NotificationService) as NotificationManager;
            notificationManager?.Notify(Constants.NotificationId, notification);
        }
    }
}