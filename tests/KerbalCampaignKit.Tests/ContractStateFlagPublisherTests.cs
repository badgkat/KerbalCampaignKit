using Xunit;
using KerbalCampaignKit.Triggers.Events;
using KerbalDialogueKit.Flags;

namespace KerbalCampaignKit.Tests
{
    public class ContractStateFlagPublisherTests
    {
        [Fact]
        public void Publish_SetsCompleteFlag_WhenStateIsComplete()
        {
            var flags = new FlagStore();
            ContractStateFlagPublisher.Publish(flags, "BKEX_RoverMonolith", "complete");
            Assert.Equal("true", flags.Get("contract:BKEX_RoverMonolith.complete"));
        }

        [Fact]
        public void Publish_SetsAcceptedFlag_WhenStateIsAccepted()
        {
            var flags = new FlagStore();
            ContractStateFlagPublisher.Publish(flags, "BKEX_FlyToIsland", "accepted");
            Assert.Equal("true", flags.Get("contract:BKEX_FlyToIsland.accepted"));
        }

        [Fact]
        public void Publish_DoesNothing_WhenFlagsNull()
        {
            // Should not throw.
            ContractStateFlagPublisher.Publish(null, "BKEX_X", "complete");
        }

        [Fact]
        public void Publish_DoesNothing_WhenContractNameNullOrEmpty()
        {
            var flags = new FlagStore();
            ContractStateFlagPublisher.Publish(flags, null, "complete");
            ContractStateFlagPublisher.Publish(flags, "", "complete");
            // No flag should have been set.
            Assert.Null(flags.Get("contract:.complete"));
            Assert.Null(flags.Get("contract:null.complete"));
        }

        [Fact]
        public void Publish_DoesNothing_WhenStateNullOrEmpty()
        {
            var flags = new FlagStore();
            ContractStateFlagPublisher.Publish(flags, "BKEX_X", null);
            ContractStateFlagPublisher.Publish(flags, "BKEX_X", "");
            // No state-suffixed flag should have been set.
            Assert.Null(flags.Get("contract:BKEX_X."));
            Assert.Null(flags.Get("contract:BKEX_X.null"));
        }
    }
}
