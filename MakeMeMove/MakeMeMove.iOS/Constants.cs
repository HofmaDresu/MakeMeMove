using System.Collections.Generic;

namespace MakeMeMove.iOS
{
    public static class Constants
    {
        public const string DatabaseName = "MakeMeMove.db3";
		public const string ExerciseNotificationCategoryId = "EXERCISE_CATEGORY";
		public const string NextId = "NEXT_ACTION";
		public const string CompleteId = "COMPLETE_ACTION";
		public const string ExerciseName = "ExerciseName";
		public const string ExerciseQuantity = "ExerciseQuantity";

        public enum NotificationSounds
        {
            Bloom,
            Calypso,
            ChooChoo,
            Fanfare,
            Ladder,
            Noir,
            SherwoodForest,
            Aurora,
            Bamboo,
            Circles,
            Complete, 
            Hello,
            Input,
            Keys,
            Note,
            Popcorn,
            Synth,
            Telegraph,
            Tiptoes
        }

        public static readonly Dictionary<NotificationSounds, string> NotificaitonSoundsMap = new Dictionary<NotificationSounds, string>
        {
            { NotificationSounds.Bloom, "Bloom.caf" },
            { NotificationSounds.Calypso, "Calypso.caf" },
            { NotificationSounds.ChooChoo, "Choo_Choo.caf" },
            { NotificationSounds.Fanfare, "Fanfare.caf" },
            { NotificationSounds.Ladder, "Ladder.caf" },
            { NotificationSounds.Noir, "Noir.caf" },
            { NotificationSounds.SherwoodForest, "Sherwood_Forest.caf" },
            { NotificationSounds.Aurora, "sms_alert_Aurora.caf" },
            { NotificationSounds.Bamboo, "sms_alert_Bamboo.caf" },
            { NotificationSounds.Circles, "sms_alert_Circles.caf" },
            { NotificationSounds.Complete, "sms_alert_Complete.caf" },
            { NotificationSounds.Hello, "sms_alert_Hello.caf" },
            { NotificationSounds.Input, "sms_alert_Input.caf" },
            { NotificationSounds.Keys, "sms_alert_Keys.caf" },
            { NotificationSounds.Note, "sms_alert_Note.caf" },
            { NotificationSounds.Popcorn, "sms_alert_Popcorn.caf" },
            { NotificationSounds.Synth, "sms_alert_Synth.caf" },
            { NotificationSounds.Telegraph, "Telegraph.caf" },
            { NotificationSounds.Tiptoes, "Tiptoes.caf" }
        };
    }
}