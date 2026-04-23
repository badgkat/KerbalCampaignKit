using ContractConfigurator;
using Contracts;
using KerbalDialogueKit.Core;

namespace KerbalCampaignKit.Contracts
{
    public class FlagNotEqualsRequirement : ContractRequirement
    {
        protected string flagName;
        protected string flagValue;

        public override bool LoadFromConfig(ConfigNode configNode)
        {
            bool valid = base.LoadFromConfig(configNode);
            valid &= ConfigNodeUtil.ParseValue<string>(configNode, "name", x => flagName = x, this);
            valid &= ConfigNodeUtil.ParseValue<string>(configNode, "value", x => flagValue = x, this);
            return valid;
        }

        public override bool RequirementMet(ConfiguredContract contract)
        {
            var actual = DialogueKit.Flags?.Get(flagName) ?? string.Empty;
            return actual != flagValue;
        }

        public override void OnSave(ConfigNode configNode)
        {
            configNode.AddValue("name", flagName);
            configNode.AddValue("value", flagValue);
        }

        public override void OnLoad(ConfigNode configNode)
        {
            flagName = configNode.GetValue("name");
            flagValue = configNode.GetValue("value");
        }

        protected override string RequirementText()
        {
            return $"Flag '{flagName}' must not equal '{flagValue}'";
        }
    }
}
