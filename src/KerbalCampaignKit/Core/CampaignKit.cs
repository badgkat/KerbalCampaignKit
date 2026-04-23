using KerbalCampaignKit.Chapters;

namespace KerbalCampaignKit.Core
{
    /// <summary>
    /// Public static entry point. Consumer mods call these methods to
    /// interact with KerbalCampaignKit.
    /// </summary>
    public static class CampaignKit
    {
        public static ChapterManager Chapters { get; internal set; }
    }
}
