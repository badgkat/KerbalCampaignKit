using System.Collections.Generic;

namespace KerbalCampaignKit.Config
{
    /// <summary>
    /// Abstracts a cfg node for loaders so they can be unit-tested without
    /// KSP's sealed ConfigNode. Wrap a real ConfigNode via ConfigNodeAdapter.
    /// </summary>
    public interface ISceneNode
    {
        string GetValue(string key);
        IEnumerable<string> GetValues(string key);
        IEnumerable<ISceneNode> GetNodes(string name);
        bool HasValue(string key);
        bool HasNode(string name);
    }

    public sealed class ConfigNodeAdapter : ISceneNode
    {
        private readonly ConfigNode node;
        public ConfigNodeAdapter(ConfigNode node) { this.node = node; }
        public string GetValue(string key) => node.GetValue(key);
        public IEnumerable<string> GetValues(string key) => node.GetValues(key);
        public IEnumerable<ISceneNode> GetNodes(string name)
        {
            foreach (var child in node.GetNodes(name))
                yield return new ConfigNodeAdapter(child);
        }
        public bool HasValue(string key) => node.HasValue(key);
        public bool HasNode(string name) => node.HasNode(name);
    }
}
