using KerbalCampaignKit.Config;

namespace KerbalCampaignKit.Reputation
{
    public static class ReputationLoader
    {
        public static ReputationIncome LoadIncome(ISceneNode node)
        {
            var inc = new ReputationIncome();
            inc.Enabled = !node.HasValue("enabled") || node.GetValue("enabled") == "true";
            if (node.HasValue("intervalDays") && double.TryParse(node.GetValue("intervalDays"), out var i))
                inc.IntervalDays = i;

            foreach (var tierNode in node.GetNodes("TIER"))
            {
                var tier = new ReputationIncome.Tier();
                if (tierNode.HasValue("min") && double.TryParse(tierNode.GetValue("min"), out var min))
                    tier.Min = min;
                if (tierNode.HasValue("max") && double.TryParse(tierNode.GetValue("max"), out var max))
                    tier.Max = max;
                if (tierNode.HasValue("income") && double.TryParse(tierNode.GetValue("income"), out var income))
                    tier.Income = income;
                tier.Label = tierNode.HasValue("label") ? tierNode.GetValue("label") : null;
                inc.Tiers.Add(tier);
            }
            return inc;
        }

        public static ReputationDecay LoadDecay(ISceneNode node)
        {
            var d = new ReputationDecay();
            d.Enabled = !node.HasValue("enabled") || node.GetValue("enabled") == "true";
            if (node.HasValue("ratePercentPerMonth") && double.TryParse(node.GetValue("ratePercentPerMonth"), out var r))
                d.RatePercentPerMonth = r;
            if (node.HasValue("resetOnContractComplete"))
                d.ResetOnContractComplete = node.GetValue("resetOnContractComplete") == "true";
            if (node.HasValue("tierFloors"))
                d.TierFloors = node.GetValue("tierFloors") == "true";
            return d;
        }
    }
}
