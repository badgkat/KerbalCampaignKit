using KerbalCampaignKit.Config;

namespace KerbalCampaignKit.Chapters
{
    public static class ChapterLoader
    {
        public static Chapter Load(ISceneNode node)
        {
            if (!node.HasValue("id")) return null;
            var id = node.GetValue("id");
            if (string.IsNullOrEmpty(id)) return null;

            return new Chapter
            {
                Id = id,
                Name = node.HasValue("name") ? node.GetValue("name") : id,
                Description = node.HasValue("description") ? node.GetValue("description") : string.Empty,
                EntryTriggerId = node.HasValue("ENTRY_TRIGGER") ? node.GetValue("ENTRY_TRIGGER") : null,
                ExitTriggerId = node.HasValue("EXIT_TRIGGER") ? node.GetValue("EXIT_TRIGGER") : null,
            };
        }
    }
}
