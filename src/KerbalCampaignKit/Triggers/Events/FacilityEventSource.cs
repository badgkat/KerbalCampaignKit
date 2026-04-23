using System;
using Upgradeables;

namespace KerbalCampaignKit.Triggers.Events
{
    public sealed class FacilityEventSource : IEventSource
    {
        private Action<EventRecord> sink;

        public void Register(Action<EventRecord> onEvent)
        {
            sink = onEvent;
            GameEvents.OnKSCFacilityUpgraded.Add(OnUpgraded);
            GameEvents.OnKSCFacilityUpgrading.Add(OnUpgrading);
        }

        public void Unregister()
        {
            GameEvents.OnKSCFacilityUpgraded.Remove(OnUpgraded);
            GameEvents.OnKSCFacilityUpgrading.Remove(OnUpgrading);
            sink = null;
        }

        private void OnUpgraded(UpgradeableFacility f, int level)
        {
            if (sink == null || f == null) return;
            var record = new EventRecord { Type = EventType.FacilityUpgraded };
            record.Params["facility"] = f.name;
            record.Params["level"] = level.ToString();
            sink(record);
        }

        private void OnUpgrading(UpgradeableFacility f, int level) { }

        public void FireEntered(string facility)
        {
            if (sink == null) return;
            var record = new EventRecord { Type = EventType.FacilityEntered };
            record.Params["facility"] = facility;
            sink(record);
        }
    }
}
