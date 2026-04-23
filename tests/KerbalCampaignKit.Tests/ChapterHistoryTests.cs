using KerbalCampaignKit.Chapters;
using Xunit;

namespace KerbalCampaignKit.Tests
{
    public class ChapterHistoryTests
    {
        [Fact]
        public void RecordsEntries_InOrder()
        {
            var h = new ChapterHistory();
            h.RecordEntry("1", 0);
            h.RecordEntry("2", 500);
            h.RecordEntry("3", 1500);

            Assert.Equal(3, h.Entries.Count);
            Assert.Equal("1", h.Entries[0].ChapterId);
            Assert.Equal(0, h.Entries[0].TimestampSeconds);
            Assert.Equal("3", h.Entries[2].ChapterId);
        }

        [Fact]
        public void HasEntered_TrueIfAnyEntryMatches()
        {
            var h = new ChapterHistory();
            h.RecordEntry("1", 0);
            h.RecordEntry("2", 500);

            Assert.True(h.HasEntered("1"));
            Assert.True(h.HasEntered("2"));
            Assert.False(h.HasEntered("3"));
        }
    }
}
