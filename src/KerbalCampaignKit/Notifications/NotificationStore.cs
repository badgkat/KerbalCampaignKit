using System.Collections.Generic;

namespace KerbalCampaignKit.Notifications
{
    public sealed class NotificationStore
    {
        private readonly List<Notification> items = new List<Notification>();

        public IReadOnlyList<Notification> All => items;

        public void Add(Notification n)
        {
            items.Add(n);
            CampaignKitEvents.FireNotificationAdded(n);
        }

        public List<Notification> At(string target)
        {
            var result = new List<Notification>();
            foreach (var n in items)
                if (n.Target == target) result.Add(n);
            return result;
        }

        public List<Notification> AtOrBelow(string target)
        {
            var result = new List<Notification>();
            foreach (var n in items)
                if (IsAtOrBelow(n.Target, target)) result.Add(n);
            return result;
        }

        public NotificationSeverity? Highest(string target)
        {
            var any = false;
            var highest = NotificationSeverity.Info;
            foreach (var n in items)
            {
                if (!IsAtOrBelow(n.Target, target)) continue;
                any = true;
                if (n.Severity == NotificationSeverity.Action) return NotificationSeverity.Action;
            }
            return any ? (NotificationSeverity?)highest : null;
        }

        public void Clear(string target, string source = null)
        {
            var removed = items.FindAll(n => n.Target == target && (source == null || n.Source == source));
            items.RemoveAll(n => n.Target == target && (source == null || n.Source == source));
            foreach (var r in removed) CampaignKitEvents.FireNotificationCleared(r);
        }

        public void ClearAll(string targetPrefix)
        {
            var removed = items.FindAll(n => IsAtOrBelow(n.Target, targetPrefix));
            items.RemoveAll(n => IsAtOrBelow(n.Target, targetPrefix));
            foreach (var r in removed) CampaignKitEvents.FireNotificationCleared(r);
        }

        public bool HasAny(string target)
        {
            foreach (var n in items)
                if (IsAtOrBelow(n.Target, target)) return true;
            return false;
        }

        private static bool IsAtOrBelow(string candidate, string ancestor)
        {
            if (candidate == ancestor) return true;
            if (candidate == null || ancestor == null) return false;
            return candidate.StartsWith(ancestor + ".");
        }
    }
}
