using KerbalCampaignKit.Tests.TestHelpers;
using KerbalCampaignKit.Triggers;
using KerbalCampaignKit.Triggers.Events;
using Xunit;

namespace KerbalCampaignKit.Tests
{
    public class TriggerLoaderTests
    {
        [Fact]
        public void ParsesIdAndOnceFlag()
        {
            var node = new FakeConfigNode()
                .Add("id", "chapter_4_on_duna")
                .Add("once", "false");

            var trigger = TriggerLoader.Load(node);
            Assert.Equal("chapter_4_on_duna", trigger.Id);
            Assert.False(trigger.Once);
        }

        [Fact]
        public void OnceDefaultsTrue()
        {
            var node = new FakeConfigNode().Add("id", "t1");
            Assert.True(TriggerLoader.Load(node).Once);
        }

        [Fact]
        public void ParsesEvent_WithParams()
        {
            var eventNode = new FakeConfigNode()
                .Add("type", "ContractComplete")
                .Add("contract", "BKEX_ProbeFlyby_Duna");
            var node = new FakeConfigNode()
                .Add("id", "t1")
                .AddNode("ON_EVENT", eventNode);

            var t = TriggerLoader.Load(node);
            Assert.Single(t.Events);
            Assert.Equal(EventType.ContractComplete, t.Events[0].Type);
            Assert.Equal("BKEX_ProbeFlyby_Duna", t.Events[0].ParamMatch["contract"]);
        }

        [Fact]
        public void ParsesWhenExpressionFromNestedNode()
        {
            var when = new FakeConfigNode().Add("flagExpression", "chapter == 3");
            var node = new FakeConfigNode()
                .Add("id", "t1")
                .AddNode("WHEN", when);

            var t = TriggerLoader.Load(node);
            Assert.Equal("chapter == 3", t.WhenExpression);
        }

        [Fact]
        public void ParsesActions_InOrder()
        {
            var setFlag = new FakeConfigNode()
                .Add("name", "foo")
                .Add("value", "bar");
            var advance = new FakeConfigNode().Add("target", "4");
            var actions = new FakeConfigNode()
                .AddNode("SET_FLAG", setFlag)
                .AddNode("ADVANCE_CHAPTER", advance);
            var node = new FakeConfigNode()
                .Add("id", "t1")
                .AddNode("ACTIONS", actions);

            var t = TriggerLoader.Load(node);
            Assert.Equal(2, t.Actions.Count);
            Assert.Equal("SET_FLAG", t.Actions[0].Kind);
            Assert.Equal("foo", t.Actions[0].Params["name"]);
            Assert.Equal("ADVANCE_CHAPTER", t.Actions[1].Kind);
            Assert.Equal("4", t.Actions[1].Params["target"]);
        }

        [Fact]
        public void ReturnsNull_IfIdMissing()
        {
            var node = new FakeConfigNode();
            Assert.Null(TriggerLoader.Load(node));
        }
    }
}
