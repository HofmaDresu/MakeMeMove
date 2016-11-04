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

        public static void CreateNotification(Data data, Context context)
        {
            var nextExercise = data.GetNextEnabledExercise();
            CreateNotification(data, context, nextExercise);
        }

        public static void CreateNotification(Data data, Context context, ExerciseBlock nextExercise)
        {
            if (nextExercise == null) return;
            var userIsPremium = data.UserIsPremium();

            var completedIntent = new Intent(context, typeof(CompleteExerciseBroadcastReceiver));
            completedIntent.PutExtra(Constants.ExerciseName, nextExercise.CombinedName);
            completedIntent.PutExtra(Constants.ExerciseQuantity, nextExercise.Quantity);
            var completedPendingIntent = PendingIntent.GetBroadcast(context, DateTime.Now.Millisecond, completedIntent, PendingIntentFlags.OneShot);
            
            var nextIntent = new Intent(context, typeof(ChangeExerciseBroadcastReceiver));
            nextIntent.PutExtra(Constants.ExerciseName, nextExercise.CombinedName);
            nextIntent.PutExtra(Constants.ExerciseQuantity, nextExercise.Quantity);
            var nextPendingIntent = PendingIntent.GetBroadcast(context, DateTime.Now.Millisecond, nextIntent, PendingIntentFlags.CancelCurrent);

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

            var timeToMoveMessage = string.Format(context.Resources.GetString(Resource.String.TimeToMoveMessage), nextExercise.Quantity, nextExercise.CombinedName);
            var builder = new NotificationCompat.Builder(context)
                .SetContentTitle(context.Resources.GetString(Resource.String.TimeToMoveTitle))
                .SetContentText(timeToMoveMessage)
                .SetDefaults(NotificationCompat.DefaultSound | NotificationCompat.DefaultVibrate)
                .SetContentIntent(clickPendingIntent);


            var changeExerciseButtonText = context.Resources.GetString(Resource.String.ChangeExerciseButtonText);
            var completedButtonText = context.Resources.GetString(Resource.String.CompletedButtonText);

            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                builder
                    .SetPriority((int) NotificationPriority.High)
                    .SetVisibility(NotificationCompat.VisibilityPublic)
                    .SetCategory("reminder")
                    .SetSmallIcon(Resource.Drawable.Mmm_white_icon)
                    .SetColor(Color.Rgb(215, 78, 10))
                    .AddAction(new NotificationCompat.Action(Resource.Drawable.ic_shuffle_black_24dp, changeExerciseButtonText, nextPendingIntent));
                if (userIsPremium)
                {
                    builder
                        .AddAction(new NotificationCompat.Action(Resource.Drawable.ic_done_black_24dp, completedButtonText, completedPendingIntent));
                }
            }
            else if ((int)Build.VERSION.SdkInt >= 20)
            {
                builder
                    .SetSmallIcon(Resource.Drawable.icon)
                    .AddAction(new NotificationCompat.Action(Resource.Drawable.ic_shuffle_black_24dp, changeExerciseButtonText, nextPendingIntent));
                if (userIsPremium)
                {
                    builder
                        .AddAction(new NotificationCompat.Action(Resource.Drawable.ic_done_black_24dp, completedButtonText, completedPendingIntent));
                }
            }
            else
            {
                // Disable obsolete warning 'cause this was how you do it pre-20
#pragma warning disable 618
                builder
                    .SetSmallIcon(Resource.Drawable.icon)
                    .AddAction(Resource.Drawable.ic_shuffle_white_24dp, changeExerciseButtonText, nextPendingIntent);
                if (userIsPremium)
                {
                    builder
                        .AddAction(Resource.Drawable.ic_done_white_24dp, completedButtonText, completedPendingIntent);
                }
#pragma warning restore 618
            }

            var notification = builder.Build();

            notification.Flags |= NotificationFlags.AutoCancel;

            var notificationManager = NotificationManagerCompat.From(context);
            notificationManager?.Notify(Constants.NotificationId, notification);
        }
    }
}