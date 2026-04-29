using System;
using Contracts;

namespace KerbalCampaignKit.Triggers.Events
{
    public sealed class ContractEventSource : IEventSource
    {
        private Action<EventRecord> sink;

        public void Register(Action<EventRecord> onEvent)
        {
            sink = onEvent;
            GameEvents.Contract.onCompleted.Add(OnCompleted);
            GameEvents.Contract.onAccepted.Add(OnAccepted);
            GameEvents.Contract.onFailed.Add(OnFailed);
            GameEvents.Contract.onCancelled.Add(OnCancelled);
        }

        public void Unregister()
        {
            GameEvents.Contract.onCompleted.Remove(OnCompleted);
            GameEvents.Contract.onAccepted.Remove(OnAccepted);
            GameEvents.Contract.onFailed.Remove(OnFailed);
            GameEvents.Contract.onCancelled.Remove(OnCancelled);
            sink = null;
        }

        private void OnCompleted(Contract c) => Fire(EventType.ContractComplete, c);
        private void OnAccepted(Contract c)  => Fire(EventType.ContractAccepted, c);
        private void OnFailed(Contract c)    => Fire(EventType.ContractFailed, c);
        private void OnCancelled(Contract c) => Fire(EventType.ContractCancelled, c);

        private void Fire(EventType type, Contract contract)
        {
            if (sink == null || contract == null) return;
            var name = ContractTypeName(contract);
            var record = new EventRecord { Type = type };
            record.Params["contract"] = name;
            sink(record);

            var state = StateForType(type);
            ContractStateFlagPublisher.Publish(KerbalDialogueKit.Core.DialogueKit.Flags, name, state);
        }

        private static string StateForType(EventType type)
        {
            switch (type)
            {
                case EventType.ContractAccepted:  return "accepted";
                case EventType.ContractComplete:  return "complete";
                case EventType.ContractFailed:    return "failed";
                case EventType.ContractCancelled: return "cancelled";
                default: return null;
            }
        }

        private static string ContractTypeName(Contract c)
        {
            var configuredType = c.GetType();
            var prop = configuredType.GetProperty("contractType");
            if (prop != null)
            {
                var val = prop.GetValue(c, null);
                if (val != null)
                {
                    var nameProp = val.GetType().GetProperty("name");
                    if (nameProp != null)
                        return (string)nameProp.GetValue(val, null);
                }
            }
            return configuredType.Name;
        }
    }
}
