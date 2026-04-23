using Xunit;

namespace KerbalCampaignKit.Tests
{
    public class SmokeTest
    {
        [Fact]
        public void CanReferenceCampaignKit()
        {
            var type = typeof(KerbalCampaignKit.Core.CampaignKit);
            Assert.NotNull(type);
        }
    }
}
