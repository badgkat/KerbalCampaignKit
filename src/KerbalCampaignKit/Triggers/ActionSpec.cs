using System.Collections.Generic;

namespace KerbalCampaignKit.Triggers
{
    public sealed class ActionSpec
    {
        public string Kind;
        public Dictionary<string, string> Params = new Dictionary<string, string>();

        public string Get(string key) => Params.TryGetValue(key, out var v) ? v : null;
    }
}
