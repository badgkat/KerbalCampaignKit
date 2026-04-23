using System.Collections.Generic;

namespace KerbalCampaignKit.Notifications
{
    public sealed class AutoClearWatcher
    {
        private readonly NotificationStore store;

        public AutoClearWatcher(NotificationStore store) { this.store = store; }

        public void OnSceneEnded(string sceneId)
        {
            var toRemove = new List<Notification>();
            foreach (var n in store.All)
                if (n.ClearOn == NotificationClearOn.SceneEnded && n.ClearSceneId == sceneId)
                    toRemove.Add(n);
            foreach (var n in toRemove) store.Clear(n.Target, n.Source);
        }

        public void OnFacilityEntered(string facility)
        {
            var toRemove = new List<Notification>();
            foreach (var n in store.All)
            {
                if (n.ClearOn != NotificationClearOn.FacilityEntered) continue;
                if (TargetIsAtOrBelow(n.Target, facility)) toRemove.Add(n);
            }
            foreach (var n in toRemove) store.Clear(n.Target, n.Source);
        }

        public void OnFlagSet(string name, string value)
        {
            var toRemove = new List<Notification>();
            foreach (var n in store.All)
            {
                if (n.ClearOn != NotificationClearOn.FlagSet) continue;
                if (n.ClearFlag != name) continue;
                if (n.ClearFlagValue != null && n.ClearFlagValue != value) continue;
                toRemove.Add(n);
            }
            foreach (var n in toRemove) store.Clear(n.Target, n.Source);
        }

        private static bool TargetIsAtOrBelow(string candidate, string ancestor)
        {
            if (candidate == ancestor) return true;
            if (candidate == null || ancestor == null) return false;
            return candidate.StartsWith(ancestor + ".");
        }
    }
}
