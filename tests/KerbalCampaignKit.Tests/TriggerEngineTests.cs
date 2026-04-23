using KerbalCampaignKit.Chapters;
using KerbalCampaignKit.Notifications;
using KerbalCampaignKit.PendingScenes;
using KerbalCampaignKit.Tests.TestHelpers;
using KerbalCampaignKit.Triggers;
using KerbalCampaignKit.Triggers.Actions;
using KerbalCampaignKit.Triggers.Events;
using KerbalDialogueKit.Flags;
using Xunit;

namespace KerbalCampaignKit.Tests
{
    public class TriggerEngineTests
    {
        private static (TriggerEngine engine, ActionContext ctx) NewEngine()
        {
            var ctx = new ActionContext
            {
                Flags = new FlagStore(),
                Chapters = new ChapterManager(),
                Notifications = new NotificationStore(),
                PendingScenes = new PendingSceneQueue(),
                Currencies = new FakeCurrencyAdapter(),
                NowSeconds = 0
            };
            var engine = new TriggerEngine(ctx);
            engine.RegisterAction(new SetFlagAction());
            engine.RegisterAction(new AdvanceChapterAction());
            return (engine, ctx);
        }

        private static Trigger TriggerMatchingContract(string id, string contractName)
        {
            var t = new Trigger { Id = id };
            var ev = new EventSpec { Type = EventType.ContractComplete };
            ev.ParamMatch["contract"] = contractName;
            t.Events.Add(ev);
            return t;
        }

        [Fact]
        public void Dispatch_FiresMatchingTrigger()
        {
            var (engine, ctx) = NewEngine();
            var trigger = TriggerMatchingContract("t1", "BKEX_FlybyDuna");
            trigger.Actions.Add(new ActionSpec { Kind = "SET_FLAG" });
            trigger.Actions[0].Params["name"] = "fired";
            trigger.Actions[0].Params["value"] = "true";
            engine.Register(trigger);

            var record = new EventRecord { Type = EventType.ContractComplete };
            record.Params["contract"] = "BKEX_FlybyDuna";
            engine.Dispatch(record);

            Assert.Equal("true", ctx.Flags.Get("fired"));
        }

        [Fact]
        public void Dispatch_SkipsNonMatchingTriggers()
        {
            var (engine, ctx) = NewEngine();
            var trigger = TriggerMatchingContract("t1", "BKEX_FlybyDuna");
            trigger.Actions.Add(new ActionSpec { Kind = "SET_FLAG" });
            trigger.Actions[0].Params["name"] = "fired";
            trigger.Actions[0].Params["value"] = "true";
            engine.Register(trigger);

            var record = new EventRecord { Type = EventType.ContractComplete };
            record.Params["contract"] = "DifferentContract";
            engine.Dispatch(record);

            Assert.Null(ctx.Flags.Get("fired"));
        }

        [Fact]
        public void Dispatch_HonorsWhenExpression()
        {
            var (engine, ctx) = NewEngine();
            var trigger = TriggerMatchingContract("t1", "X");
            trigger.WhenExpression = "chapter == 3";
            trigger.Actions.Add(new ActionSpec { Kind = "SET_FLAG" });
            trigger.Actions[0].Params["name"] = "fired";
            trigger.Actions[0].Params["value"] = "true";
            engine.Register(trigger);

            var record = new EventRecord { Type = EventType.ContractComplete };
            record.Params["contract"] = "X";
            engine.Dispatch(record);
            Assert.Null(ctx.Flags.Get("fired"));

            ctx.Flags.Set("chapter", "3");
            engine.Dispatch(record);
            Assert.Equal("true", ctx.Flags.Get("fired"));
        }

        [Fact]
        public void OnceTrigger_DoesNotFireTwice()
        {
            var (engine, ctx) = NewEngine();
            var trigger = TriggerMatchingContract("t1", "X");
            trigger.Once = true;
            trigger.Actions.Add(new ActionSpec { Kind = "SET_FLAG" });
            trigger.Actions[0].Params["name"] = "count";
            trigger.Actions[0].Params["value"] = "1";
            engine.Register(trigger);

            var record = new EventRecord { Type = EventType.ContractComplete };
            record.Params["contract"] = "X";
            engine.Dispatch(record);
            ctx.Flags.Set("count", "old");
            engine.Dispatch(record);
            Assert.Equal("old", ctx.Flags.Get("count"));
        }

        [Fact]
        public void RepeatableTrigger_FiresEveryTime()
        {
            var (engine, ctx) = NewEngine();
            var trigger = TriggerMatchingContract("t1", "X");
            trigger.Once = false;
            trigger.Actions.Add(new ActionSpec { Kind = "SET_FLAG" });
            trigger.Actions[0].Params["name"] = "last";
            trigger.Actions[0].Params["value"] = "fired";
            engine.Register(trigger);

            var record = new EventRecord { Type = EventType.ContractComplete };
            record.Params["contract"] = "X";
            engine.Dispatch(record);
            ctx.Flags.Set("last", "overwritten");
            engine.Dispatch(record);
            Assert.Equal("fired", ctx.Flags.Get("last"));
        }

        [Fact]
        public void UnregisteredActionKind_SkipsSilently()
        {
            var (engine, _) = NewEngine();
            var trigger = TriggerMatchingContract("t1", "X");
            trigger.Actions.Add(new ActionSpec { Kind = "UNKNOWN_ACTION" });
            engine.Register(trigger);

            var record = new EventRecord { Type = EventType.ContractComplete };
            record.Params["contract"] = "X";
            engine.Dispatch(record);  // should not throw
        }
    }
}
