using System.Collections.Generic;
using KerbalCampaignKit.Config;

namespace KerbalCampaignKit.Tests.TestHelpers
{
    public sealed class FakeConfigNode : ISceneNode
    {
        private readonly List<(string key, string value)> values = new List<(string, string)>();
        private readonly List<(string name, FakeConfigNode node)> nodes = new List<(string, FakeConfigNode)>();

        public FakeConfigNode Add(string key, string value) { values.Add((key, value)); return this; }
        public FakeConfigNode AddNode(string name, FakeConfigNode node) { nodes.Add((name, node)); return this; }

        public string GetValue(string key)
        {
            foreach (var (k, v) in values) if (k == key) return v;
            return null;
        }
        public IEnumerable<string> GetValues(string key)
        {
            foreach (var (k, v) in values) if (k == key) yield return v;
        }
        public IEnumerable<ISceneNode> GetNodes(string name)
        {
            foreach (var (n, node) in nodes) if (n == name) yield return node;
        }
        public bool HasValue(string key)
        {
            foreach (var (k, _) in values) if (k == key) return true;
            return false;
        }
        public bool HasNode(string name)
        {
            foreach (var (n, _) in nodes) if (n == name) return true;
            return false;
        }
    }
}
