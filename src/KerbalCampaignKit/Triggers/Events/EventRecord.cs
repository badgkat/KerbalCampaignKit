using System.Collections.Generic;

namespace KerbalCampaignKit.Triggers.Events
{
    public sealed class EventRecord
    {
        public EventType Type;
        public Dictionary<string, string> Params = new Dictionary<string, string>();

        public string Get(string key)
        {
            return Params.TryGetValue(key, out var v) ? v : null;
        }
    }
}
