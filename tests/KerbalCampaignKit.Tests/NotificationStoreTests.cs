using KerbalCampaignKit.Notifications;
using Xunit;

namespace KerbalCampaignKit.Tests
{
    public class NotificationStoreTests
    {
        private static Notification Make(string target, NotificationSeverity sev, string source)
            => new Notification { Target = target, Severity = sev, Source = source };

        [Fact]
        public void At_ReturnsOnlyExactMatches()
        {
            var store = new NotificationStore();
            store.Add(Make("admin", NotificationSeverity.Action, "x"));
            store.Add(Make("admin.desk", NotificationSeverity.Info, "y"));

            var at = store.At("admin");
            Assert.Single(at);
            Assert.Equal("x", at[0].Source);
        }

        [Fact]
        public void AtOrBelow_ReturnsExactAndDescendants()
        {
            var store = new NotificationStore();
            store.Add(Make("admin", NotificationSeverity.Action, "x"));
            store.Add(Make("admin.desk", NotificationSeverity.Info, "y"));
            store.Add(Make("admin.desk.contracts", NotificationSeverity.Info, "z"));
            store.Add(Make("tracking", NotificationSeverity.Info, "w"));

            var below = store.AtOrBelow("admin");
            Assert.Equal(3, below.Count);
        }

        [Fact]
        public void AtOrBelow_DoesNotMatchPrefixesOfOtherNames()
        {
            // "admin" must NOT match "administration" — segment-aware.
            var store = new NotificationStore();
            store.Add(Make("administration", NotificationSeverity.Action, "a"));

            Assert.Empty(store.AtOrBelow("admin"));
        }

        [Fact]
        public void Highest_ReturnsActionOverInfo()
        {
            var store = new NotificationStore();
            store.Add(Make("admin", NotificationSeverity.Info, "x"));
            store.Add(Make("admin.desk", NotificationSeverity.Action, "y"));

            Assert.Equal(NotificationSeverity.Action, store.Highest("admin"));
        }

        [Fact]
        public void Highest_NullWhenNoneFound()
        {
            var store = new NotificationStore();
            Assert.Null(store.Highest("admin"));
        }

        [Fact]
        public void Clear_BySourceRemovesOnlyMatch()
        {
            var store = new NotificationStore();
            store.Add(Make("admin", NotificationSeverity.Action, "x"));
            store.Add(Make("admin", NotificationSeverity.Info, "y"));

            store.Clear("admin", "x");
            var remaining = store.At("admin");
            Assert.Single(remaining);
            Assert.Equal("y", remaining[0].Source);
        }

        [Fact]
        public void ClearAll_RemovesDescendants()
        {
            var store = new NotificationStore();
            store.Add(Make("admin", NotificationSeverity.Action, "x"));
            store.Add(Make("admin.desk", NotificationSeverity.Info, "y"));
            store.Add(Make("tracking", NotificationSeverity.Info, "z"));

            store.ClearAll("admin");
            Assert.Empty(store.AtOrBelow("admin"));
            Assert.Single(store.At("tracking"));
        }

        [Fact]
        public void FiresAddedEvent()
        {
            Notification seen = null;
            System.Action<Notification> handler = n => seen = n;
            CampaignKitEvents.OnNotificationAdded += handler;
            try
            {
                var store = new NotificationStore();
                var note = Make("admin", NotificationSeverity.Info, "x");
                store.Add(note);
                Assert.Same(note, seen);
            }
            finally
            {
                CampaignKitEvents.OnNotificationAdded -= handler;
            }
        }
    }
}
