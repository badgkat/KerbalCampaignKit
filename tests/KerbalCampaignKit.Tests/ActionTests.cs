using KerbalCampaignKit.Chapters;
using KerbalCampaignKit.Notifications;
using KerbalCampaignKit.PendingScenes;
using KerbalCampaignKit.Tests.TestHelpers;
using KerbalCampaignKit.Triggers;
using KerbalCampaignKit.Triggers.Actions;
using KerbalDialogueKit.Flags;
using Xunit;

namespace KerbalCampaignKit.Tests
{
    public class ActionTests
    {
        private static ActionContext NewCtx()
        {
            return new ActionContext
            {
                Flags = new FlagStore(),
                Chapters = new ChapterManager(),
                Notifications = new NotificationStore(),
                PendingScenes = new PendingSceneQueue(),
                Currencies = new FakeCurrencyAdapter(),
                NowSeconds = 1000
            };
        }

        [Fact]
        public void SetFlag_WritesToFlagStore()
        {
            var ctx = NewCtx();
            var spec = new ActionSpec { Kind = "SET_FLAG" };
            spec.Params["name"] = "mood";
            spec.Params["value"] = "happy";

            new SetFlagAction().Execute(spec, ctx);
            Assert.Equal("happy", ctx.Flags.Get("mood"));
        }

        [Fact]
        public void ClearFlag_Removes()
        {
            var ctx = NewCtx();
            ctx.Flags.Set("mood", "happy");
            var spec = new ActionSpec { Kind = "CLEAR_FLAG" };
            spec.Params["name"] = "mood";

            new ClearFlagAction().Execute(spec, ctx);
            Assert.Null(ctx.Flags.Get("mood"));
        }

        [Fact]
        public void AdvanceChapter_UpdatesManager()
        {
            var ctx = NewCtx();
            var spec = new ActionSpec { Kind = "ADVANCE_CHAPTER" };
            spec.Params["target"] = "3";

            new AdvanceChapterAction().Execute(spec, ctx);
            Assert.Equal("3", ctx.Chapters.Current);
        }

        [Fact]
        public void AdjustFunds_DelegatesToCurrencyAdapter()
        {
            var ctx = NewCtx();
            var spec = new ActionSpec { Kind = "ADJUST_FUNDS" };
            spec.Params["amount"] = "5000";

            new AdjustCurrencyAction("ADJUST_FUNDS").Execute(spec, ctx);
            Assert.Equal(5000, ((FakeCurrencyAdapter)ctx.Currencies).Funds);
        }

        [Fact]
        public void AdjustReputation_DelegatesToCurrencyAdapter()
        {
            var ctx = NewCtx();
            var spec = new ActionSpec { Kind = "ADJUST_REPUTATION" };
            spec.Params["amount"] = "-25";

            new AdjustCurrencyAction("ADJUST_REPUTATION").Execute(spec, ctx);
            Assert.Equal(-25, ((FakeCurrencyAdapter)ctx.Currencies).Reputation);
        }

        [Fact]
        public void EnqueueScene_Immediate_CallsEnqueuer()
        {
            var ctx = NewCtx();
            var fake = new FakeSceneEnqueuer();
            ctx.SceneEnqueuer = fake;
            var spec = new ActionSpec { Kind = "ENQUEUE_SCENE" };
            spec.Params["sceneId"] = "my_scene";

            new EnqueueSceneAction().Execute(spec, ctx);
            Assert.Contains("my_scene", fake.Enqueued);
            Assert.Empty(ctx.PendingScenes.Pending);
        }

        [Fact]
        public void EnqueueScene_OnFacilityEnter_QueuesNotImmediate()
        {
            var ctx = NewCtx();
            var fake = new FakeSceneEnqueuer();
            ctx.SceneEnqueuer = fake;
            var spec = new ActionSpec { Kind = "ENQUEUE_SCENE" };
            spec.Params["sceneId"] = "my_scene";
            spec.Params["when"] = "OnFacilityEnter";
            spec.Params["facility"] = "Administration";

            new EnqueueSceneAction().Execute(spec, ctx);
            Assert.Empty(fake.Enqueued);
            Assert.Single(ctx.PendingScenes.Pending);
            Assert.Equal("Administration", ctx.PendingScenes.Pending[0].Facility);
        }

        [Fact]
        public void Notify_AddsToStoreWithSeverity()
        {
            var ctx = NewCtx();
            var spec = new ActionSpec { Kind = "NOTIFY" };
            spec.Params["target"] = "admin";
            spec.Params["severity"] = "Action";
            spec.Params["source"] = "chapter_4_directive";

            new NotifyAction().Execute(spec, ctx);
            var found = ctx.Notifications.At("admin");
            Assert.Single(found);
            Assert.Equal(NotificationSeverity.Action, found[0].Severity);
            Assert.Equal("chapter_4_directive", found[0].Source);
        }

        [Fact]
        public void ClearNotification_RemovesMatchingSource()
        {
            var ctx = NewCtx();
            ctx.Notifications.Add(new Notification
            {
                Target = "admin",
                Severity = NotificationSeverity.Action,
                Source = "x"
            });
            var spec = new ActionSpec { Kind = "CLEAR_NOTIFICATION" };
            spec.Params["target"] = "admin";
            spec.Params["source"] = "x";

            new ClearNotificationAction().Execute(spec, ctx);
            Assert.Empty(ctx.Notifications.At("admin"));
        }
    }
}
