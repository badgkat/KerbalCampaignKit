using KerbalCampaignKit.Chapters;
using Xunit;

namespace KerbalCampaignKit.Tests
{
    public class ChapterManagerTests
    {
        [Fact]
        public void InitiallyNoChapter()
        {
            var m = new ChapterManager();
            Assert.Null(m.Current);
        }

        [Fact]
        public void AdvanceSetsCurrentAndRecordsHistory()
        {
            var m = new ChapterManager();
            m.Advance("1", 0);
            m.Advance("2", 500);

            Assert.Equal("2", m.Current);
            Assert.Equal(2, m.History.Entries.Count);
            Assert.True(m.HasEntered("1"));
            Assert.True(m.HasEntered("2"));
        }

        [Theory]
        [InlineData("1", "1", true)]
        [InlineData("2", "1", true)]
        [InlineData("1", "2", false)]
        public void IsAtLeast_NumericComparison(string current, string threshold, bool expected)
        {
            var m = new ChapterManager();
            m.Advance(current, 0);
            Assert.Equal(expected, m.IsAtLeast(threshold));
        }

        [Fact]
        public void IsAtLeast_NonNumeric_ExactMatchOnly()
        {
            var m = new ChapterManager();
            m.Advance("act_interplanetary", 0);
            Assert.True(m.IsAtLeast("act_interplanetary"));
            Assert.False(m.IsAtLeast("act_endgame"));
        }

        [Fact]
        public void FiresCallback_OnAdvance()
        {
            var m = new ChapterManager();
            string fromSeen = "UNSET", toSeen = "UNSET";
            m.OnChapterChanged += (from, to) => { fromSeen = from; toSeen = to; };

            m.Advance("1", 0);
            Assert.Null(fromSeen);
            Assert.Equal("1", toSeen);

            m.Advance("2", 0);
            Assert.Equal("1", fromSeen);
            Assert.Equal("2", toSeen);
        }
    }
}
