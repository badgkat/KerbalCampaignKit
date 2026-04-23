using System.Collections.Generic;
using KerbalCampaignKit.Triggers.Actions;
using KerbalCampaignKit.Triggers.Events;

namespace KerbalCampaignKit.Triggers
{
    public sealed class TriggerEngine
    {
        private readonly ActionContext ctx;
        private readonly List<Trigger> triggers = new List<Trigger>();
        private readonly HashSet<string> firedOnceIds = new HashSet<string>();
        private readonly Dictionary<string, IAction> actions =
            new Dictionary<string, IAction>();

        public TriggerEngine(ActionContext ctx) { this.ctx = ctx; }

        public void Register(Trigger trigger) => triggers.Add(trigger);

        public void RegisterAction(IAction action) => actions[action.Kind] = action;

        public IReadOnlyCollection<string> FiredOnceIds => firedOnceIds;

        public void MarkFired(string triggerId) => firedOnceIds.Add(triggerId);

        public void Dispatch(EventRecord record)
        {
            foreach (var trigger in triggers)
            {
                if (trigger.Once && firedOnceIds.Contains(trigger.Id)) continue;
                if (!AnyEventMatches(trigger, record)) continue;
                if (!RequirementExpression.Evaluate(trigger.WhenExpression, ctx.Flags)) continue;

                foreach (var spec in trigger.Actions)
                {
                    if (!actions.TryGetValue(spec.Kind, out var action)) continue;
                    action.Execute(spec, ctx);
                }

                if (trigger.Once) firedOnceIds.Add(trigger.Id);
            }
        }

        private static bool AnyEventMatches(Trigger trigger, EventRecord record)
        {
            foreach (var spec in trigger.Events)
                if (spec.Matches(record)) return true;
            return false;
        }
    }
}
