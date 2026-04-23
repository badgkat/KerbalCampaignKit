using System.Collections.Generic;
using UnityEngine;

namespace KerbalCampaignKit.Triggers.Actions
{
    public sealed class PublishEventAction : IAction
    {
        public string Kind => "PUBLISH_EVENT";

        private static readonly Dictionary<string, EventData<Dictionary<string, string>>> registered
            = new Dictionary<string, EventData<Dictionary<string, string>>>();

        public void Execute(ActionSpec spec, ActionContext ctx)
        {
            var name = spec.Get("eventName") ?? spec.Get("name");
            if (string.IsNullOrEmpty(name)) return;

            if (!registered.TryGetValue(name, out var ev))
            {
                ev = new EventData<Dictionary<string, string>>(name);
                registered[name] = ev;
            }

            var payload = new Dictionary<string, string>();
            foreach (var kvp in spec.Params)
                if (kvp.Key != "eventName" && kvp.Key != "name")
                    payload[kvp.Key] = kvp.Value;

            ev.Fire(payload);
        }
    }
}
