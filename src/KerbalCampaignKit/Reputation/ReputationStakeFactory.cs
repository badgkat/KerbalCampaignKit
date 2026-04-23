using ContractConfigurator;
using Contracts;

namespace KerbalCampaignKit.Reputation
{
    public class ReputationStakeFactory : BehaviourFactory
    {
        protected float successBonus;
        protected float failurePenalty;

        public override bool Load(ConfigNode configNode)
        {
            bool valid = base.Load(configNode);
            valid &= ConfigNodeUtil.ParseValue<float>(configNode, "successBonus", x => successBonus = x, this, 0f);
            valid &= ConfigNodeUtil.ParseValue<float>(configNode, "failurePenalty", x => failurePenalty = x, this, 0f);
            return valid;
        }

        public override ContractBehaviour Generate(ConfiguredContract contract)
        {
            return new ReputationStakeBehaviour(successBonus, failurePenalty);
        }
    }
}
