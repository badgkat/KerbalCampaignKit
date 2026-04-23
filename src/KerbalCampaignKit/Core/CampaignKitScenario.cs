using System.Collections.Generic;
using KerbalCampaignKit.Chapters;
using KerbalCampaignKit.Notifications;
using KerbalCampaignKit.PendingScenes;
using KerbalCampaignKit.Reputation;
using KerbalCampaignKit.Triggers;

namespace KerbalCampaignKit.Core
{
    [KSPScenario(ScenarioCreationOptions.AddToAllGames,
        GameScenes.SPACECENTER, GameScenes.FLIGHT, GameScenes.TRACKSTATION, GameScenes.EDITOR)]
    public class CampaignKitScenario : ScenarioModule
    {
        public static CampaignKitScenario Instance;

        public ChapterManager Chapters = new ChapterManager();
        public NotificationStore Notifications = new NotificationStore();
        public PendingSceneQueue PendingScenes = new PendingSceneQueue();
        public ReputationEconomy Reputation = new ReputationEconomy();
        public TriggerEngine Engine;  // wired by addon after scenario loads

        // OnLoad runs before the addon wires the engine, so buffer fired-once IDs
        // and let the addon replay them via ApplyBufferedFiredTriggers() after wiring.
        private readonly List<string> bufferedFiredTriggerIds = new List<string>();

        public override void OnAwake()
        {
            base.OnAwake();
            Instance = this;
        }

        public void ApplyBufferedFiredTriggers()
        {
            if (Engine == null) return;
            foreach (var id in bufferedFiredTriggerIds) Engine.MarkFired(id);
            bufferedFiredTriggerIds.Clear();
        }

        public override void OnSave(ConfigNode node)
        {
            base.OnSave(node);
            node.AddValue("currentChapter", Chapters.Current ?? string.Empty);

            var history = node.AddNode("CHAPTER_HISTORY");
            foreach (var entry in Chapters.History.Entries)
            {
                var e = history.AddNode("ENTERED");
                e.AddValue("chapter", entry.ChapterId);
                e.AddValue("timestamp", entry.TimestampSeconds);
            }

            var fired = node.AddNode("FIRED_TRIGGERS");
            if (Engine != null)
                foreach (var id in Engine.FiredOnceIds)
                    fired.AddValue("trigger", id);

            var pending = node.AddNode("PENDING_SCENES");
            foreach (var ps in PendingScenes.Pending)
            {
                var s = pending.AddNode("SCENE");
                s.AddValue("sceneId", ps.SceneId);
                if (!string.IsNullOrEmpty(ps.Facility)) s.AddValue("facility", ps.Facility);
                if (!string.IsNullOrEmpty(ps.FromTriggerId)) s.AddValue("fromTrigger", ps.FromTriggerId);
            }

            var notes = node.AddNode("NOTIFICATIONS");
            foreach (var n in Notifications.All)
            {
                var nn = notes.AddNode("NOTIFICATION");
                nn.AddValue("target", n.Target);
                nn.AddValue("severity", n.Severity.ToString());
                nn.AddValue("source", n.Source ?? string.Empty);
                nn.AddValue("clearOn", n.ClearOn.ToString());
                if (!string.IsNullOrEmpty(n.ClearSceneId)) nn.AddValue("clearSceneId", n.ClearSceneId);
                if (!string.IsNullOrEmpty(n.ClearFlag)) nn.AddValue("clearFlag", n.ClearFlag);
                if (!string.IsNullOrEmpty(n.ClearFlagValue)) nn.AddValue("clearFlagValue", n.ClearFlagValue);
                nn.AddValue("addedAt", n.AddedAtSeconds);
            }

            var rep = node.AddNode("REPUTATION_STATE");
            rep.AddValue("lastIncomeTime", Reputation.LastIncomeTimeSeconds);
            rep.AddValue("lastDecayTime", Reputation.LastDecayTimeSeconds);
            rep.AddValue("decayHaltUntil", Reputation.Decay.HaltUntilSeconds);
            rep.AddValue("highestRepReached", Reputation.HighestRepReached);
        }

        public override void OnLoad(ConfigNode node)
        {
            base.OnLoad(node);
            var current = node.GetValue("currentChapter");
            if (!string.IsNullOrEmpty(current))
                Chapters.Advance(current, 0);

            if (node.HasNode("CHAPTER_HISTORY"))
            {
                Chapters.History.Entries.Clear();
                foreach (var e in node.GetNode("CHAPTER_HISTORY").GetNodes("ENTERED"))
                {
                    double.TryParse(e.GetValue("timestamp"), out var ts);
                    Chapters.History.RecordEntry(e.GetValue("chapter"), ts);
                }
            }

            if (node.HasNode("FIRED_TRIGGERS"))
            {
                bufferedFiredTriggerIds.Clear();
                foreach (var id in node.GetNode("FIRED_TRIGGERS").GetValues("trigger"))
                    bufferedFiredTriggerIds.Add(id);
                if (Engine != null) ApplyBufferedFiredTriggers();
            }

            if (node.HasNode("PENDING_SCENES"))
            {
                foreach (var s in node.GetNode("PENDING_SCENES").GetNodes("SCENE"))
                {
                    PendingScenes.Add(new PendingScene
                    {
                        SceneId = s.GetValue("sceneId"),
                        Facility = s.HasValue("facility") ? s.GetValue("facility") : null,
                        FromTriggerId = s.HasValue("fromTrigger") ? s.GetValue("fromTrigger") : null,
                    });
                }
            }

            if (node.HasNode("NOTIFICATIONS"))
            {
                foreach (var nn in node.GetNode("NOTIFICATIONS").GetNodes("NOTIFICATION"))
                {
                    var n = new Notification
                    {
                        Target = nn.GetValue("target"),
                        Source = nn.GetValue("source"),
                        ClearSceneId = nn.HasValue("clearSceneId") ? nn.GetValue("clearSceneId") : null,
                        ClearFlag = nn.HasValue("clearFlag") ? nn.GetValue("clearFlag") : null,
                        ClearFlagValue = nn.HasValue("clearFlagValue") ? nn.GetValue("clearFlagValue") : null,
                    };
                    System.Enum.TryParse(nn.GetValue("severity"), out NotificationSeverity sev);
                    n.Severity = sev;
                    System.Enum.TryParse(nn.GetValue("clearOn"), out NotificationClearOn clearOn);
                    n.ClearOn = clearOn;
                    double.TryParse(nn.GetValue("addedAt"), out n.AddedAtSeconds);
                    Notifications.Add(n);
                }
            }

            if (node.HasNode("REPUTATION_STATE"))
            {
                var rep = node.GetNode("REPUTATION_STATE");
                double.TryParse(rep.GetValue("lastIncomeTime"), out Reputation.LastIncomeTimeSeconds);
                double.TryParse(rep.GetValue("lastDecayTime"), out Reputation.LastDecayTimeSeconds);
                double.TryParse(rep.GetValue("decayHaltUntil"), out Reputation.Decay.HaltUntilSeconds);
                double.TryParse(rep.GetValue("highestRepReached"), out Reputation.HighestRepReached);
            }
        }
    }
}
