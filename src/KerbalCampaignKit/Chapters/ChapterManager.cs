using System;
using KerbalCampaignKit.Notifications;

namespace KerbalCampaignKit.Chapters
{
    public sealed class ChapterManager
    {
        public string Current { get; private set; }
        public ChapterHistory History { get; } = new ChapterHistory();

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
