using System.Collections.Generic;

namespace KerbalCampaignKit.Notifications
{
    public sealed class NotificationStore
    {
        private readonly List<Notification> items = new List<Notification>();

        public IReadOnlyList<Notification> All => items;

        public void Add(Notification n) => items.Add(n);

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
            items.RemoveAll(n => n.Target == target && (source == null || n.Source == source));
        }

        public void ClearAll(string targetPrefix)
        {
            items.RemoveAll(n => IsAtOrBelow(n.Target, targetPrefix));
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
