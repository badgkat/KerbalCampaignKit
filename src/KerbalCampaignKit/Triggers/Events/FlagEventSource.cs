using System;

namespace KerbalCampaignKit.Triggers.Events
{
    public sealed class FlagEventSource : IEventSource
    {
        private Action<EventRecord> sink;
        public void Register(Action<EventRecord> onEvent) { sink = onEvent; }
        public void Unregister() { sink = null; }

        public void FireChanged(string name, string value)
        {
            if (sink == null) return;
            var record = new EventRecord { Type = EventType.FlagChanged };
            record.Params["name"] = name;
            record.Params["value"] = value ?? string.Empty;
            sink(record);
        }
    }
}
