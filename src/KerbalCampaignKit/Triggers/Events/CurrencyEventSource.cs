using System;

namespace KerbalCampaignKit.Triggers.Events
{
    public sealed class CurrencyEventSource : IEventSource
    {
        private Action<EventRecord> sink;
        private double lastRep = -1, lastFunds = -1, lastScience = -1;

        public void Register(Action<EventRecord> onEvent)
        {
            sink = onEvent;
            GameEvents.OnReputationChanged.Add(OnRepChanged);
            GameEvents.OnFundsChanged.Add(OnFundsChanged);
            GameEvents.OnScienceChanged.Add(OnScienceChanged);

            if (global::Reputation.Instance != null) lastRep = global::Reputation.Instance.reputation;
            if (Funding.Instance != null) lastFunds = Funding.Instance.Funds;
            if (ResearchAndDevelopment.Instance != null) lastScience = ResearchAndDevelopment.Instance.Science;
        }

        public void Unregister()
        {
            GameEvents.OnReputationChanged.Remove(OnRepChanged);
            GameEvents.OnFundsChanged.Remove(OnFundsChanged);
            GameEvents.OnScienceChanged.Remove(OnScienceChanged);
            sink = null;
        }

        private void OnRepChanged(float newValue, TransactionReasons reason)
            => FireCrossed(EventType.ReputationCrossed, lastRep, newValue, ref lastRep);

        private void OnFundsChanged(double newValue, TransactionReasons reason)
            => FireCrossed(EventType.FundsCrossed, lastFunds, newValue, ref lastFunds);

        private void OnScienceChanged(float newValue, TransactionReasons reason)
            => FireCrossed(EventType.ScienceCrossed, lastScience, newValue, ref lastScience);

        private void FireCrossed(EventType type, double oldValue, double newValue, ref double storage)
        {
            if (sink == null)
            {
                storage = newValue;
                return;
            }
            var record = new EventRecord { Type = type };
            record.Params["value"] = newValue.ToString("F0");
            record.Params["direction"] = newValue > oldValue ? "Rising" : "Falling";
            sink(record);
            storage = newValue;
        }
    }
}
