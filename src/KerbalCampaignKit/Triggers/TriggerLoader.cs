using System;
using KerbalCampaignKit.Config;
using KerbalCampaignKit.Triggers.Events;

namespace KerbalCampaignKit.Triggers
{
    public static class TriggerLoader
    {
        public static Trigger Load(ISceneNode node)
        {
            if (!node.HasValue("id")) return null;

            var trigger = new Trigger
            {
                Id = node.GetValue("id"),
                Once = !node.HasValue("once") || node.GetValue("once") != "false",
            };

            foreach (var eventNode in node.GetNodes("ON_EVENT"))
            {
                var typeText = eventNode.GetValue("type");
                if (string.IsNullOrEmpty(typeText)) continue;
                if (!Enum.TryParse<EventType>(typeText, out var evType)) continue;
                var spec = new EventSpec { Type = evType };
                foreach (var key in KnownEventParams)
                {
                    if (eventNode.HasValue(key))
                        spec.ParamMatch[key] = eventNode.GetValue(key);
                }
                trigger.Events.Add(spec);
            }

            if (node.HasNode("WHEN"))
            {
                foreach (var when in node.GetNodes("WHEN"))
                {
                    if (when.HasValue("flagExpression"))
                    {
                        trigger.WhenExpression = when.GetValue("flagExpression");
                        break;
                    }
                }
            }

            if (node.HasNode("ACTIONS"))
            {
                foreach (var actions in node.GetNodes("ACTIONS"))
                {
                    foreach (var actionKind in KnownActionKinds)
                    {
                        foreach (var actionNode in actions.GetNodes(actionKind))
                        {
                            var spec = new ActionSpec { Kind = actionKind };
                            foreach (var key in KnownActionParams)
                            {
                                if (actionNode.HasValue(key))
                                    spec.Params[key] = actionNode.GetValue(key);
                            }
                            trigger.Actions.Add(spec);
                        }
                    }
                    break;  // only first ACTIONS block
                }
            }

            return trigger;
        }

        private static readonly string[] KnownEventParams = {
            "contract", "vesselType", "kerbalName", "trait", "level", "body",
            "name", "value", "threshold", "direction", "facility", "chapter",
            "days", "ref", "sceneId", "choiceId", "eventName"
        };

        private static readonly string[] KnownActionKinds = {
            "SET_FLAG", "CLEAR_FLAG", "ENQUEUE_SCENE", "ADVANCE_CHAPTER",
            "ADJUST_FUNDS", "ADJUST_REPUTATION", "ADJUST_SCIENCE",
            "NOTIFY", "CLEAR_NOTIFICATION", "PUBLISH_EVENT"
        };

        private static readonly string[] KnownActionParams = {
            "name", "value", "sceneId", "when", "facility", "target",
            "amount", "reason", "severity", "source", "clearOn",
            "clearSceneId", "clearFlag", "clearFlagValue", "eventName"
        };
    }
}
