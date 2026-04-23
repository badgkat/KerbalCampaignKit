using System;
using KerbalCampaignKit.Chapters;

namespace KerbalCampaignKit.Triggers.Events
{
    public sealed class ChapterEventSource : IEventSource
    {
        private readonly ChapterManager chapters;
        private Action<EventRecord> sink;

        public ChapterEventSource(ChapterManager chapters) { this.chapters = chapters; }

        public void Register(Action<EventRecord> onEvent)
        {
            sink = onEvent;
            chapters.OnChapterChanged += OnChanged;
        }

        public void Unregister()
        {
            chapters.OnChapterChanged -= OnChanged;
            sink = null;
        }

        private void OnChanged(string from, string to)
        {
            if (sink == null) return;
            if (!string.IsNullOrEmpty(from))
            {
                var exit = new EventRecord { Type = EventType.ChapterExited };
                exit.Params["chapter"] = from;
                sink(exit);
            }
            if (!string.IsNullOrEmpty(to))
            {
                var enter = new EventRecord { Type = EventType.ChapterEntered };
                enter.Params["chapter"] = to;
                sink(enter);
            }
        }
    }
}
