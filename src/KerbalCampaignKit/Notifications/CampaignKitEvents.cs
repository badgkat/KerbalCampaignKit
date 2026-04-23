using System;

namespace KerbalCampaignKit.Notifications
{
    public static class CampaignKitEvents
    {
        public static event Action<Notification> OnNotificationAdded;
        public static event Action<Notification> OnNotificationCleared;
        /// <summary>Args: (fromChapter, toChapter). fromChapter null on first entry.</summary>
        public static event Action<string, string> OnChapterChanged;
        /// <summary>Args: (amountPaid).</summary>
        public static event Action<double> OnReputationIncomeTick;

        internal static void FireNotificationAdded(Notification n) => OnNotificationAdded?.Invoke(n);
        internal static void FireNotificationCleared(Notification n) => OnNotificationCleared?.Invoke(n);
        internal static void FireChapterChanged(string from, string to) => OnChapterChanged?.Invoke(from, to);
        internal static void FireReputationIncomeTick(double amount) => OnReputationIncomeTick?.Invoke(amount);
    }
}
