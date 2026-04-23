using System.Collections.Generic;
using KerbalCampaignKit.Triggers.Actions;

namespace KerbalCampaignKit.Tests.TestHelpers
{
    public sealed class FakeSceneEnqueuer : ISceneEnqueuer
    {
        public List<string> Enqueued { get; } = new List<string>();

        public bool EnqueueById(string sceneId)
        {
            Enqueued.Add(sceneId);
            return true;
        }
    }
}
