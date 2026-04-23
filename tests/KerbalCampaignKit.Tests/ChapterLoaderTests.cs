using KerbalCampaignKit.Chapters;
using KerbalCampaignKit.Tests.TestHelpers;
using Xunit;

namespace KerbalCampaignKit.Tests
{
    public class ChapterLoaderTests
    {
        [Fact]
        public void ParsesBasicChapter()
        {
            var node = new FakeConfigNode()
                .Add("id", "3")
                .Add("name", "Beyond the Mun")
                .Add("description", "The anomaly signals stretch past Kerbin's moons.")
                .Add("ENTRY_TRIGGER", "chapter_3_entry")
                .Add("EXIT_TRIGGER", "chapter_3_exit");

            var chapter = ChapterLoader.Load(node);

            Assert.Equal("3", chapter.Id);
            Assert.Equal("Beyond the Mun", chapter.Name);
            Assert.Equal("The anomaly signals stretch past Kerbin's moons.", chapter.Description);
            Assert.Equal("chapter_3_entry", chapter.EntryTriggerId);
            Assert.Equal("chapter_3_exit", chapter.ExitTriggerId);
        }

        [Fact]
        public void AllowsMissingExitTrigger()
        {
            var node = new FakeConfigNode()
                .Add("id", "1")
                .Add("name", "First Steps")
                .Add("ENTRY_TRIGGER", "chapter_1_startup");

            var chapter = ChapterLoader.Load(node);
            Assert.Null(chapter.ExitTriggerId);
        }

        [Fact]
        public void ReturnsNullForMissingId()
        {
            var node = new FakeConfigNode().Add("name", "Nameless");
            var chapter = ChapterLoader.Load(node);
            Assert.Null(chapter);
        }
    }
}
