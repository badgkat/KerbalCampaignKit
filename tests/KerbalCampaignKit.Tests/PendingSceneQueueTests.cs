using KerbalCampaignKit.PendingScenes;
using Xunit;

namespace KerbalCampaignKit.Tests
{
    public class PendingSceneQueueTests
    {
        [Fact]
        public void TakeForFacility_ReturnsAndRemovesMatching()
        {
            var q = new PendingSceneQueue();
            q.Add(new PendingScene { SceneId = "a", Facility = "Administration" });
            q.Add(new PendingScene { SceneId = "b", Facility = "MissionControl" });
            q.Add(new PendingScene { SceneId = "c", Facility = "Administration" });

            var taken = q.TakeForFacility("Administration");
            Assert.Equal(2, taken.Count);
            Assert.Single(q.Pending);
            Assert.Equal("MissionControl", q.Pending[0].Facility);
        }

        [Fact]
        public void TakeImmediate_TakesNullFacility()
        {
            var q = new PendingSceneQueue();
            q.Add(new PendingScene { SceneId = "a", Facility = null });
            q.Add(new PendingScene { SceneId = "b", Facility = "Administration" });

            var taken = q.TakeImmediate();
            Assert.Single(taken);
            Assert.Equal("a", taken[0].SceneId);
        }
    }
}
