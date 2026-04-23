using ContractConfigurator;
using Contracts;

namespace KerbalCampaignKit.Contracts
{
    public class ReputationMinimumRequirement : ContractRequirement
    {
        protected double minRep;

        public override bool LoadFromConfig(ConfigNode configNode)
        {
            bool valid = base.LoadFromConfig(configNode);
            valid &= ConfigNodeUtil.ParseValue<double>(configNode, "value", x => minRep = x, this);
            return valid;
        }

        public override bool RequirementMet(ConfiguredContract contract)
        {
            // Disambiguate: KerbalCampaignKit.Reputation namespace shadows KSP's Reputation class.
            if (global::Reputation.Instance == null) return false;
            return global::Reputation.Instance.reputation >= minRep;
        }

        public override void OnSave(ConfigNode configNode)
        {
            configNode.AddValue("value", minRep);
        }

        public override void OnLoad(ConfigNode configNode)
        {
            double.TryParse(configNode.GetValue("value"), out minRep);
        }

        protected override string RequirementText()
        {
            return $"Requires at least {minRep:F0} reputation";
        }
    }
}
