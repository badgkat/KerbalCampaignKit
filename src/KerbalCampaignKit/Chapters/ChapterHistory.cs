using System.Collections.Generic;

namespace KerbalCampaignKit.Chapters
{
    public sealed class ChapterHistory
    {
        public struct Entry
        {
            public string ChapterId;
            public double TimestampSeconds;
        }

        public List<Entry> Entries { get; } = new List<Entry>();

        public void RecordEntry(string chapterId, double timestampSeconds)
        {
            Entries.Add(new Entry { ChapterId = chapterId, TimestampSeconds = timestampSeconds });
        }

        public bool HasEntered(string chapterId)
        {
            foreach (var e in Entries) if (e.ChapterId == chapterId) return true;
            return false;
        }
    }
}
