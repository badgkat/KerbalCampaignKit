using KerbalCampaignKit.Reputation;
using Xunit;

namespace KerbalCampaignKit.Tests
{
    public class ReputationDecayTests
    {
        [Fact]
        public void NoDecayIfDisabled()
        {
            var d = new ReputationDecay { Enabled = false, RatePercentPerMonth = 10 };
            Assert.Equal(0, d.ComputeDecay(currentRep: 500, elapsedDays: 30, floor: 0));
        }

        [Fact]
        public void DecayScalesWithElapsedTime()
        {
            var d = new ReputationDecay { Enabled = true, RatePercentPerMonth = 2 };
            Assert.Equal(10, d.ComputeDecay(currentRep: 500, elapsedDays: 30, floor: 0));
            Assert.Equal(20, d.ComputeDecay(currentRep: 500, elapsedDays: 60, floor: 0));
        }

        [Fact]
        public void DecayRespectsFloor()
        {
            var d = new ReputationDecay { Enabled = true, RatePercentPerMonth = 50 };
            Assert.Equal(50, d.ComputeDecay(currentRep: 300, elapsedDays: 30, floor: 250));
        }

        [Fact]
        public void DecayIsZeroBelowFloor()
        {
            var d = new ReputationDecay { Enabled = true, RatePercentPerMonth = 50 };
            Assert.Equal(0, d.ComputeDecay(currentRep: 200, elapsedDays: 30, floor: 250));
        }

        [Fact]
        public void DecayIsHaltedIfHaltUntilInFuture()
        {
            var d = new ReputationDecay { Enabled = true, RatePercentPerMonth = 10, HaltUntilSeconds = 1000 };
            Assert.True(d.IsHalted(nowSeconds: 500));
            Assert.False(d.IsHalted(nowSeconds: 1500));
        }
    }
}
