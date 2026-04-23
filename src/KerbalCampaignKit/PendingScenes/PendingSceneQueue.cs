using System.Collections.Generic;

namespace KerbalCampaignKit.PendingScenes
{
    public sealed class PendingSceneQueue
    {
        private readonly List<PendingScene> items = new List<PendingScene>();

        public IReadOnlyList<PendingScene> Pending => items;

        public void Add(PendingScene scene) => items.Add(scene);

        public List<PendingScene> TakeForFacility(string facility)
        {
            var matched = new List<PendingScene>();
            foreach (var s in items)
                if (s.Facility == facility) matched.Add(s);
            items.RemoveAll(s => s.Facility == facility);
            return matched;
        }

        public List<PendingScene> TakeImmediate()
        {
            var matched = new List<PendingScene>();
            foreach (var s in items)
                if (string.IsNullOrEmpty(s.Facility)) matched.Add(s);
            items.RemoveAll(s => string.IsNullOrEmpty(s.Facility));
            return matched;
        }

        public void Clear() => items.Clear();
    }
}
