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
        public const string UserDefaultsNotificationSoundsKey = "USER_DEFAULTS_NOTIFICATION_SOUND";

        public enum NotificationSounds
        {
            SystemDefault,
            Aurora,
            Bamboo,
            Bloom,
            Calypso,
            ChooChoo,
            Circles,
            Complete, 
            Fanfare,
            Hello,
            Input,
            Keys,
            Ladder,
            Noir,
            Note,
            Popcorn,
            SherwoodForest,
            Synth,
            Telegraph,
            Tiptoes
        }

        public static readonly Dictionary<NotificationSounds, string> NotificaitonSoundsMap = new Dictionary<NotificationSounds, string>
        {
            { NotificationSounds.SystemDefault, "" },
            { NotificationSounds.Bloom, "Bloom.caf" },
            { NotificationSounds.Calypso, "Calypso.caf" },
            { NotificationSounds.ChooChoo, "Choo_Choo.caf" },
            { NotificationSounds.Fanfare, "Fanfare.caf" },
            { NotificationSounds.Ladder, "Ladder.caf" },
            { NotificationSounds.Noir, "Noir.caf" },
            { NotificationSounds.SherwoodForest, "Sherwood_Forest.caf" },
            { NotificationSounds.Aurora, "sms_alert_aurora.caf" },
            { NotificationSounds.Bamboo, "sms_alert_bamboo.caf" },
            { NotificationSounds.Circles, "sms_alert_circles.caf" },
            { NotificationSounds.Complete, "sms_alert_complete.caf" },
            { NotificationSounds.Hello, "sms_alert_hello.caf" },
            { NotificationSounds.Input, "sms_alert_input.caf" },
            { NotificationSounds.Keys, "sms_alert_keys.caf" },
            { NotificationSounds.Note, "sms_alert_note.caf" },
            { NotificationSounds.Popcorn, "sms_alert_popcorn.caf" },
            { NotificationSounds.Synth, "sms_alert_synth.caf" },
            { NotificationSounds.Telegraph, "Telegraph.caf" },
            { NotificationSounds.Tiptoes, "Tiptoes.caf" }
        };
    }
}