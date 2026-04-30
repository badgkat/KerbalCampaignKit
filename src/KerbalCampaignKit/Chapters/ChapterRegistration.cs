using System.Collections.Generic;
using KerbalCampaignKit.Config;

namespace KerbalCampaignKit.Chapters
{
    /// <summary>
    /// Pure-logic helper that loads CAMPAIGN_CHAPTER cfg nodes via
    /// <see cref="ChapterLoader"/> and registers them with a
    /// <see cref="ChapterManager"/>. Split out from CampaignKitAddon so
    /// the registration loop can be unit-tested without KSP loaded.
    /// </summary>
    public static class ChapterRegistration
    {
        public static void RegisterAll(IEnumerable<ISceneNode> nodes, ChapterManager manager)
        {
            if (nodes == null || manager == null) return;
            foreach (var node in nodes)
            {
                if (node == null) continue;
                var chapter = ChapterLoader.Load(node);
                if (chapter != null) manager.RegisterDefinition(chapter);
            }
        }
    }
}
