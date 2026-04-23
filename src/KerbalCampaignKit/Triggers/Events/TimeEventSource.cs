using System;

namespace KerbalCampaignKit.Triggers.Events
{
    public sealed class TimeEventSource : IEventSource
    {
        private Action<EventRecord> sink;
        public void Register(Action<EventRecord> onEvent) { sink = onEvent; }
        public void Unregister() { sink = null; }

        public void FireElapsed(double days, string reference)
        {
            if (sink == null) return;
            var record = new EventRecord { Type = EventType.TimeElapsed };
            record.Params["days"] = days.ToString("F1");
            if (!string.IsNullOrEmpty(reference)) record.Params["ref"] = reference;
            sink(record);
        }
    }
}
