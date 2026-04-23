using KerbalCampaignKit.Reputation;
using KerbalCampaignKit.Tests.TestHelpers;
using Xunit;

namespace KerbalCampaignKit.Tests
{
    public class ReputationIncomeTests
    {
        private static ReputationIncome Build(params (double min, double max, double income)[] tiers)
        {
            var inc = new ReputationIncome { Enabled = true, IntervalDays = 30 };
            foreach (var t in tiers)
                inc.Tiers.Add(new ReputationIncome.Tier { Min = t.min, Max = t.max, Income = t.income });
            return inc;
        }

        [Fact]
        public void IncomeForRep_PicksMatchingTier()
        {
            var inc = Build((0, 100, 5000), (100, 250, 15000), (250, 500, 35000));
            Assert.Equal(5000, inc.IncomeForRep(0));
            Assert.Equal(5000, inc.IncomeForRep(99));
            Assert.Equal(15000, inc.IncomeForRep(100));
            Assert.Equal(15000, inc.IncomeForRep(249));
            Assert.Equal(35000, inc.IncomeForRep(300));
        }

        [Fact]
        public void IncomeForRep_ReturnsZeroWhenDisabled()
        {
            var inc = Build((0, 100, 5000));
            inc.Enabled = false;
            Assert.Equal(0, inc.IncomeForRep(50));
        }

        [Fact]
        public void IncomeForRep_ReturnsZeroWhenNoMatchingTier()
        {
            var inc = Build((100, 200, 5000));
            Assert.Equal(0, inc.IncomeForRep(50));
            Assert.Equal(0, inc.IncomeForRep(500));
        }

        [Fact]
        public void TierLabelForRep_ReturnsFirstMatch()
        {
            var inc = Build((0, 100, 5000), (100, 250, 15000));
            inc.Tiers[0].Label = "Startup";
            inc.Tiers[1].Label = "Established";
            Assert.Equal("Startup", inc.TierLabelForRep(50));
            Assert.Equal("Established", inc.TierLabelForRep(200));
        }

        [Fact]
        public void LoadIncome_ReadsTiers()
        {
            var root = new FakeConfigNode()
                .Add("enabled", "true")
                .Add("intervalDays", "15");
            var t1 = new FakeConfigNode()
                .Add("min", "0").Add("max", "100").Add("income", "5000").Add("label", "Startup");
            var t2 = new FakeConfigNode()
                .Add("min", "100").Add("max", "250").Add("income", "15000");
            root.AddNode("TIER", t1).AddNode("TIER", t2);

            var inc = ReputationLoader.LoadIncome(root);
            Assert.True(inc.Enabled);
            Assert.Equal(15, inc.IntervalDays);
            Assert.Equal(2, inc.Tiers.Count);
            Assert.Equal("Startup", inc.Tiers[0].Label);
            Assert.Equal(15000, inc.Tiers[1].Income);
        }
    }
}
