using System.Collections.Generic;
using KerbalCampaignKit.Triggers.Events;

namespace KerbalCampaignKit.Triggers
{
    public sealed class EventSpec
    {
        public EventType Type;
        public Dictionary<string, string> ParamMatch = new Dictionary<string, string>();

        public bool Matches(EventRecord record)
        {
            if (record.Type != Type) return false;
            foreach (var kvp in ParamMatch)
            {
                if (!record.Params.TryGetValue(kvp.Key, out var actual)) return false;
                if (actual != kvp.Value) return false;
            }
            return true;
        }
    }
}
