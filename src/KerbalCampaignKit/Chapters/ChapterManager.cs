using System;
using System.Collections.Generic;
using KerbalCampaignKit.Notifications;

namespace KerbalCampaignKit.Chapters
{
    public sealed class ChapterManager
    {
        public string Current { get; private set; }
        public ChapterHistory History { get; } = new ChapterHistory();

        // Registry of cfg-defined chapter definitions, keyed by Id.
        // Populated at startup by ChapterRegistration; consumed by code/cfg
        // that needs the human-readable name, description, or trigger ids.
        private readonly Dictionary<string, Chapter> definitions =
            new Dictionary<string, Chapter>();

        public IReadOnlyDictionary<string, Chapter> Definitions => definitions;

        public void RegisterDefinition(Chapter chapter)
        {
            if (chapter == null || string.IsNullOrEmpty(chapter.Id)) return;
            definitions[chapter.Id] = chapter;
        }

        public bool HasDefinition(string chapterId) =>
            !string.IsNullOrEmpty(chapterId) && definitions.ContainsKey(chapterId);

        public Chapter GetDefinition(string chapterId)
        {
            if (string.IsNullOrEmpty(chapterId)) return null;
            return definitions.TryGetValue(chapterId, out var c) ? c : null;
        }

        public event Action<string, string> OnChapterChanged;

        public void Advance(string chapterId, double timestampSeconds)
        {
            if (string.IsNullOrEmpty(chapterId)) return;
            var previous = Current;
            Current = chapterId;
            History.RecordEntry(chapterId, timestampSeconds);
            OnChapterChanged?.Invoke(previous, chapterId);
            CampaignKitEvents.FireChapterChanged(previous, chapterId);
        }

        public bool HasEntered(string chapterId) => History.HasEntered(chapterId);

        public bool IsAtLeast(string threshold)
        {
            if (string.IsNullOrEmpty(Current) || string.IsNullOrEmpty(threshold)) return false;
            if (int.TryParse(Current, out var cur) && int.TryParse(threshold, out var thr))
                return cur >= thr;
            return Current == threshold;
        }
    }
}
