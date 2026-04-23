using KerbalCampaignKit.Notifications;
using Xunit;

namespace KerbalCampaignKit.Tests
{
    public class AutoClearWatcherTests
    {
        [Fact]
        public void SceneEnded_ClearsMatchingNotification()
        {
            var store = new NotificationStore();
            store.Add(new Notification
            {
                Target = "admin",
                Severity = NotificationSeverity.Action,
                Source = "x",
                ClearOn = NotificationClearOn.SceneEnded,
                ClearSceneId = "directive"
            });
            var watcher = new AutoClearWatcher(store);

            watcher.OnSceneEnded("directive");
            Assert.Empty(store.At("admin"));
        }

        [Fact]
        public void SceneEnded_IgnoresNonMatchingSceneId()
        {
            var store = new NotificationStore();
            store.Add(new Notification
            {
                Target = "admin",
                ClearOn = NotificationClearOn.SceneEnded,
                ClearSceneId = "directive"
            });
            var watcher = new AutoClearWatcher(store);

            watcher.OnSceneEnded("other_scene");
            Assert.Single(store.At("admin"));
        }

        [Fact]
        public void FacilityEntered_ClearsNotificationAtThatFacility()
        {
            var store = new NotificationStore();
            store.Add(new Notification
            {
                Target = "admin",
                ClearOn = NotificationClearOn.FacilityEntered
            });
            store.Add(new Notification
            {
                Target = "admin.desk",
                ClearOn = NotificationClearOn.FacilityEntered
            });
            store.Add(new Notification
            {
                Target = "tracking",
                ClearOn = NotificationClearOn.FacilityEntered
            });

            var watcher = new AutoClearWatcher(store);
            watcher.OnFacilityEntered("admin");

            Assert.Empty(store.AtOrBelow("admin"));
            Assert.Single(store.At("tracking"));
        }

        [Fact]
        public void FlagSet_ClearsMatchingNotification()
        {
            var store = new NotificationStore();
            store.Add(new Notification
            {
                Target = "admin",
                ClearOn = NotificationClearOn.FlagSet,
                ClearFlag = "done",
                ClearFlagValue = "true"
            });
            var watcher = new AutoClearWatcher(store);

            watcher.OnFlagSet("done", "true");
            Assert.Empty(store.At("admin"));
        }

        [Fact]
        public void FlagSet_IgnoresMismatch()
        {
            var store = new NotificationStore();
            store.Add(new Notification
            {
                Target = "admin",
                ClearOn = NotificationClearOn.FlagSet,
                ClearFlag = "done",
                ClearFlagValue = "true"
            });
            var watcher = new AutoClearWatcher(store);

            watcher.OnFlagSet("done", "false");
            Assert.Single(store.At("admin"));

            watcher.OnFlagSet("other", "true");
            Assert.Single(store.At("admin"));
        }

        [Fact]
        public void ManualClearOn_NeverClearsAutomatically()
        {
            var store = new NotificationStore();
            store.Add(new Notification { Target = "admin", ClearOn = NotificationClearOn.Manual });
            var watcher = new AutoClearWatcher(store);

            watcher.OnSceneEnded("any");
            watcher.OnFacilityEntered("admin");
            watcher.OnFlagSet("anything", "true");
            Assert.Single(store.At("admin"));
        }
    }
}
