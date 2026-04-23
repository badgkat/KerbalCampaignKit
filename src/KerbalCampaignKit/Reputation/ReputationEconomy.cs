using KerbalCampaignKit.Notifications;
using KerbalCampaignKit.Triggers.Actions;

namespace KerbalCampaignKit.Reputation
{
    public sealed class ReputationEconomy
    {
        public ReputationIncome Income { get; set; } = new ReputationIncome();
        public ReputationDecay Decay { get; set; } = new ReputationDecay();

        public double LastIncomeTimeSeconds;
        public double LastDecayTimeSeconds;
        public double HighestRepReached;

        public void Tick(ICurrencyAdapter currencies, double nowSeconds)
        {
            var rep = currencies.Reputation;
            if (rep > HighestRepReached) HighestRepReached = rep;

            var intervalSeconds = Income.IntervalDays * 24 * 60 * 60;
            if (Income.Enabled && intervalSeconds > 0 && nowSeconds - LastIncomeTimeSeconds >= intervalSeconds)
            {
                var payout = Income.IncomeForRep(rep);
                if (payout > 0) currencies.AddFunds(payout);
                LastIncomeTimeSeconds = nowSeconds;
                CampaignKitEvents.FireReputationIncomeTick(payout);
            }

            if (Decay.Enabled && !Decay.IsHalted(nowSeconds))
            {
                var elapsed = nowSeconds - LastDecayTimeSeconds;
                if (elapsed > 0)
                {
                    var elapsedDays = elapsed / (24.0 * 60 * 60);
                    var floor = Decay.TierFloors ? TierFloorBelow(HighestRepReached) : 0;
                    var amount = Decay.ComputeDecay(rep, elapsedDays, floor);
                    if (amount > 0) currencies.AddReputation(-amount);
                    LastDecayTimeSeconds = nowSeconds;
                }
            }
        }

        public void HaltDecay(double nowSeconds, double days)
        {
            var until = nowSeconds + days * 24 * 60 * 60;
            if (until > Decay.HaltUntilSeconds) Decay.HaltUntilSeconds = until;
        }

        public void ResetDecayTimer(double nowSeconds)
        {
            LastDecayTimeSeconds = nowSeconds;
        }

        public double TierFloorBelow(double highestRep)
        {
            double floor = 0;
            foreach (var t in Income.Tiers)
            {
                if (highestRep >= t.Min && t.Min > floor) floor = t.Min;
            }
            return floor;
        }

        public (double value, string label)? NextGate(double currentRep)
        {
            foreach (var t in Income.Tiers)
            {
                if (t.Min > currentRep)
                    return (t.Min, t.Label ?? $">= {t.Min} reputation");
            }
            return null;
        }
    }
}
