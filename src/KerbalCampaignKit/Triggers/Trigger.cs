using System.Collections.Generic;

namespace KerbalCampaignKit.Triggers
{
    public sealed class Trigger
    {
        public string Id;
        public bool Once = true;
        public List<EventSpec> Events = new List<EventSpec>();
        public string WhenExpression;
        public List<ActionSpec> Actions = new List<ActionSpec>();
    }
}
