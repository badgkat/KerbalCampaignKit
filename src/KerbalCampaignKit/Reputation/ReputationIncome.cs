using System.Collections.Generic;

namespace KerbalCampaignKit.Reputation
{
    public sealed class ReputationIncome
    {
        public bool Enabled;
        public double IntervalDays = 30;
        public List<Tier> Tiers { get; } = new List<Tier>();

        public sealed class Tier
        {
            public double Min;
            public double Max;
            public double Income;
            public string Label;
        }

        public double IncomeForRep(double rep)
        {
            if (!Enabled) return 0;
            var t = MatchingTier(rep);
            return t?.Income ?? 0;
        }

        public string TierLabelForRep(double rep) => MatchingTier(rep)?.Label;

        private Tier MatchingTier(double rep)
        {
            foreach (var t in Tiers)
                if (rep >= t.Min && rep < t.Max) return t;
            return null;
        }
    }
}
