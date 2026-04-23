using KerbalCampaignKit.Chapters;
using KerbalCampaignKit.Notifications;
using KerbalCampaignKit.Reputation;
using KerbalCampaignKit.Triggers;
using KerbalCampaignKit.Triggers.Events;

namespace KerbalCampaignKit.Core
{
    /// <summary>
    /// Public static entry point. Consumer mods call these methods to
    /// interact with KerbalCampaignKit.
    /// </summary>
    public static class CampaignKit
    {
        public static ChapterManager Chapters { get; internal set; }
        public static NotificationStore Notifications { get; internal set; }
        public static ReputationEconomy Reputation { get; internal set; }
        public static TriggerEngine Engine { get; internal set; }

        public static void FireFacilityEntered(string facility)
        {
            facilityEventSource?.FireEntered(facility);
        }

        internal static FacilityEventSource facilityEventSource;
    }
}
