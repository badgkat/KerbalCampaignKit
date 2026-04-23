using KerbalCampaignKit.Chapters;
using KerbalCampaignKit.Notifications;
using KerbalCampaignKit.PendingScenes;
using KerbalDialogueKit.Flags;

namespace KerbalCampaignKit.Triggers.Actions
{
    public sealed class ActionContext
    {
        public FlagStore Flags;
        public ChapterManager Chapters;
        public NotificationStore Notifications;
        public PendingSceneQueue PendingScenes;
        public ICurrencyAdapter Currencies;
        public ISceneEnqueuer SceneEnqueuer;
        public double NowSeconds;
    }

    public interface ICurrencyAdapter
    {
        void AddFunds(double amount);
        void AddReputation(double amount);
        void AddScience(double amount);
        double Funds { get; }
        double Reputation { get; }
        double Science { get; }
    }

    public interface ISceneEnqueuer
    {
        bool EnqueueById(string sceneId);
    }
}
