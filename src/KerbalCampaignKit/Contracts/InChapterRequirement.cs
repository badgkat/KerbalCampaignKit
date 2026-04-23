using ContractConfigurator;
using Contracts;
using KerbalCampaignKit.Core;

namespace KerbalCampaignKit.Contracts
{
    public class InChapterRequirement : ContractRequirement
    {
        protected string chapter;

        public override bool LoadFromConfig(ConfigNode configNode)
        {
            bool valid = base.LoadFromConfig(configNode);
            valid &= ConfigNodeUtil.ParseValue<string>(configNode, "chapter", x => chapter = x, this);
            return valid;
        }

        public override bool RequirementMet(ConfiguredContract contract)
        {
            return CampaignKit.Chapters?.Current == chapter;
        }

        public override void OnSave(ConfigNode configNode)
        {
            configNode.AddValue("chapter", chapter);
        }

        public override void OnLoad(ConfigNode configNode)
        {
            chapter = configNode.GetValue("chapter");
        }

        protected override string RequirementText()
        {
            return $"Campaign must be in chapter {chapter}";
        }
    }
}
