using System;

namespace KerbalCampaignKit.Triggers.Events
{
    public sealed class VesselEventSource : IEventSource
    {
        private Action<EventRecord> sink;

        public void Register(Action<EventRecord> onEvent)
        {
            sink = onEvent;
            GameEvents.OnVesselRecoveryRequested.Add(OnRecovery);
            GameEvents.onCrash.Add(OnCrash);
        }

        public void Unregister()
        {
            GameEvents.OnVesselRecoveryRequested.Remove(OnRecovery);
            GameEvents.onCrash.Remove(OnCrash);
            sink = null;
        }

        private void OnRecovery(Vessel v)
        {
            if (sink == null || v == null) return;
            var record = new EventRecord { Type = EventType.VesselRecovered };
            record.Params["vesselType"] = v.vesselType.ToString();
            sink(record);
        }

        private void OnCrash(EventReport report)
        {
            if (sink == null) return;
            var record = new EventRecord { Type = EventType.VesselCrashed };
            if (report?.origin?.vessel != null)
                record.Params["vesselType"] = report.origin.vessel.vesselType.ToString();
            sink(record);
        }
    }
}
