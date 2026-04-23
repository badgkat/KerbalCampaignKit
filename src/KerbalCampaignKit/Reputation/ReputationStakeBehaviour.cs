using ContractConfigurator;
using Contracts;

namespace KerbalCampaignKit.Reputation
{
    public class ReputationStakeBehaviour : ContractBehaviour
    {
        public float SuccessBonus;
        public float FailurePenalty;

        public ReputationStakeBehaviour() { }

        public ReputationStakeBehaviour(float successBonus, float failurePenalty)
        {
            SuccessBonus = successBonus;
            FailurePenalty = failurePenalty;
        }

        protected override void OnCompleted()
        {
            if (SuccessBonus != 0 && global::Reputation.Instance != null)
                global::Reputation.Instance.AddReputation(SuccessBonus, TransactionReasons.ContractReward);
        }

        protected override void OnFailed()
        {
            if (FailurePenalty != 0 && global::Reputation.Instance != null)
                global::Reputation.Instance.AddReputation(-FailurePenalty, TransactionReasons.ContractPenalty);
        }

        protected override void OnSave(ConfigNode configNode)
        {
            configNode.AddValue("successBonus", SuccessBonus);
            configNode.AddValue("failurePenalty", FailurePenalty);
        }

        protected override void OnLoad(ConfigNode configNode)
        {
            float.TryParse(configNode.GetValue("successBonus"), out SuccessBonus);
            float.TryParse(configNode.GetValue("failurePenalty"), out FailurePenalty);
        }
    }
}
