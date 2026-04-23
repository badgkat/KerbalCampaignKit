namespace KerbalCampaignKit.Notifications
{
    public sealed class Notification
    {
        public string Target;
        public NotificationSeverity Severity;
        public string Source;
        public NotificationClearOn ClearOn = NotificationClearOn.Manual;
        public string ClearSceneId;
        public string ClearFlag;
        public string ClearFlagValue;
        public double AddedAtSeconds;
    }
}
