using System.Collections.Generic;
using KerbalCampaignKit.Chapters;
using KerbalCampaignKit.Config;
using KerbalCampaignKit.Tests.TestHelpers;
using Xunit;

namespace KerbalCampaignKit.Tests
{
    public class ChapterRegistrationTests
    {
        [Fact]
        public void RegisterDefinition_StoresChapterByIdOnManager()
        {
            var manager = new ChapterManager();
            var chapter = new Chapter { Id = "1", Name = "First Steps" };

            manager.RegisterDefinition(chapter);

            Assert.True(manager.HasDefinition("1"));
            Assert.Same(chapter, manager.GetDefinition("1"));
            Assert.Single(manager.Definitions);
        }

        [Fact]
        public void RegisterDefinition_NullOrMissingId_IsIgnored()
        {
            var manager = new ChapterManager();

            manager.RegisterDefinition(null);
            manager.RegisterDefinition(new Chapter { Id = null });
            manager.RegisterDefinition(new Chapter { Id = "" });

            Assert.Empty(manager.Definitions);
        }

        [Fact]
        public void RegisterDefinition_ReplacesExistingChapterWithSameId()
        {
            var manager = new ChapterManager();
            manager.RegisterDefinition(new Chapter { Id = "1", Name = "Old" });
            manager.RegisterDefinition(new Chapter { Id = "1", Name = "New" });

            Assert.Single(manager.Definitions);
            Assert.Equal("New", manager.GetDefinition("1").Name);
        }

        [Fact]
        public void RegisterAll_LoadsEachNodeAndRegistersWithManager()
        {
            var manager = new ChapterManager();
            var nodes = new List<ISceneNode>
            {
                new FakeConfigNode().Add("id", "1").Add("name", "First Steps"),
                new FakeConfigNode().Add("id", "2").Add("name", "Reaching the Mun"),
                new FakeConfigNode().Add("id", "3").Add("name", "Beyond the Mun"),
            };

            ChapterRegistration.RegisterAll(nodes, manager);

            Assert.Equal(3, manager.Definitions.Count);
            Assert.Equal("First Steps", manager.GetDefinition("1").Name);
            Assert.Equal("Reaching the Mun", manager.GetDefinition("2").Name);
            Assert.Equal("Beyond the Mun", manager.GetDefinition("3").Name);
        }

        [Fact]
        public void RegisterAll_SkipsNodesThatLoaderRejects()
        {
            var manager = new ChapterManager();
            var nodes = new List<ISceneNode>
            {
                new FakeConfigNode().Add("id", "1").Add("name", "First"),
                new FakeConfigNode().Add("name", "No id, should be skipped"),
                new FakeConfigNode().Add("id", "2").Add("name", "Second"),
            };

            ChapterRegistration.RegisterAll(nodes, manager);

            Assert.Equal(2, manager.Definitions.Count);
            Assert.True(manager.HasDefinition("1"));
            Assert.True(manager.HasDefinition("2"));
        }

        [Fact]
        public void RegisterAll_NullCollection_IsNoop()
        {
            var manager = new ChapterManager();
            ChapterRegistration.RegisterAll(null, manager);
            Assert.Empty(manager.Definitions);
        }
    }
}
