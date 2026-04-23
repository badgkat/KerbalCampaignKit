using ContractConfigurator;
using Contracts;
using KerbalCampaignKit.Triggers;
using KerbalDialogueKit.Core;

namespace KerbalCampaignKit.Contracts
{
    public class FlagExpressionRequirement : ContractRequirement
    {
        protected string expression;

        public override bool LoadFromConfig(ConfigNode configNode)
        {
            bool valid = base.LoadFromConfig(configNode);
            valid &= ConfigNodeUtil.ParseValue<string>(configNode, "expression", x => expression = x, this);
            return valid;
        }

        public override bool RequirementMet(ConfiguredContract contract)
        {
            if (DialogueKit.Flags == null) return false;
            return RequirementExpression.Evaluate(expression, DialogueKit.Flags);
        }

        public override void OnSave(ConfigNode configNode)
        {
            configNode.AddValue("expression", expression);
        }

        public override void OnLoad(ConfigNode configNode)
        {
            expression = configNode.GetValue("expression");
        }

        protected override string RequirementText()
        {
            return $"Flag expression: {expression}";
        }
    }
}
