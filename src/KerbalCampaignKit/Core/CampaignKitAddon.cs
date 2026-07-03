using System.Collections.Generic;
using KerbalCampaignKit.Chapters;
using KerbalCampaignKit.Config;
using KerbalCampaignKit.Notifications;
using KerbalCampaignKit.PendingScenes;
using KerbalCampaignKit.Reputation;
using KerbalCampaignKit.Triggers;
using KerbalCampaignKit.Triggers.Actions;
using KerbalCampaignKit.Triggers.Events;
using KerbalDialogueKit.Core;
using UnityEngine;

namespace KerbalCampaignKit.Core
{
    [KSPAddon(KSPAddon.Startup.MainMenu, true)]
    public sealed class CampaignKitAddon : MonoBehaviour
    {
        private TriggerEngine engine;
        private ActionContext ctx;
        private StateMirror mirror;
        private AutoClearWatcher autoClear;
        private readonly List<IEventSource> sources = new List<IEventSource>();

        private FlagEventSource flagSource;
        private TimeEventSource timeSource;
        private FacilityEventSource facilitySource;
        private SceneEventSource sceneSource;

        private double lastTickSeconds;

        private void Awake()
        {
            DontDestroyOnLoad(this);
            GameEvents.onLevelWasLoaded.Add(OnLevelLoaded);
        }

        private void OnDestroy()
        {
            GameEvents.onLevelWasLoaded.Remove(OnLevelLoaded);
            DialogueKit.OnAnyChoiceMade -= OnDialogueChoiceMade;
            DialogueKit.OnAnySceneEnded -= OnDialogueSceneEnded;
            foreach (var s in sources) s.Unregister();
        }

        private void OnLevelLoaded(GameScenes scene)
        {
            if (CampaignKitScenario.Instance == null) return;
            if (engine != null)
            {
                // Re-fire facility enter + play pending scenes on subsequent scene loads.
                FireFacilityForScene(scene);
                PlayPendingForScene(scene);
                return;
            }
            InitializeAll();
            FireFacilityForScene(scene);
            PlayPendingForScene(scene);
        }

        private void InitializeAll()
        {
            var scenario = CampaignKitScenario.Instance;
            ctx = new ActionContext
            {
                Flags = DialogueKit.Flags,
                Chapters = scenario.Chapters,
                Notifications = scenario.Notifications,
                PendingScenes = scenario.PendingScenes,
                Currencies = new KspCurrencyAdapter(),
                SceneEnqueuer = new KspSceneEnqueuer(),
                NowSeconds = Planetarium.GetUniversalTime()
            };

            engine = new TriggerEngine(ctx);
            scenario.Engine = engine;
            RegisterActions();

            LoadContent();

            // Replay fired-once trigger IDs that were buffered during OnLoad
            // (before the engine existed). Must run after LoadContent so the
            // triggers themselves are registered.
            scenario.ApplyBufferedFiredTriggers();

            flagSource = new FlagEventSource();
            timeSource = new TimeEventSource();
            facilitySource = new FacilityEventSource();
            sceneSource = new SceneEventSource();
            sources.Add(new ContractEventSource());
            sources.Add(new VesselEventSource());
            sources.Add(new CurrencyEventSource());
            sources.Add(facilitySource);
            sources.Add(new ChapterEventSource(scenario.Chapters));
            sources.Add(flagSource);
            sources.Add(timeSource);
            sources.Add(sceneSource);
            foreach (var s in sources) s.Register(engine.Dispatch);
            CampaignKit.facilityEventSource = facilitySource;

            autoClear = new AutoClearWatcher(scenario.Notifications);

            mirror = new StateMirror(DialogueKit.Flags, scenario.Chapters);
            mirror.SyncAll();

            CampaignKit.Chapters = scenario.Chapters;
            CampaignKit.Notifications = scenario.Notifications;
            CampaignKit.Reputation = scenario.Reputation;
            CampaignKit.Engine = engine;

            DialogueKit.OnAnyChoiceMade += OnDialogueChoiceMade;
            DialogueKit.OnAnySceneEnded += OnDialogueSceneEnded;

            lastTickSeconds = Planetarium.GetUniversalTime();
        }

        private void RegisterActions()
        {
            engine.RegisterAction(new SetFlagAction());
            engine.RegisterAction(new ClearFlagAction());
            engine.RegisterAction(new AdvanceChapterAction());
            engine.RegisterAction(new AdjustCurrencyAction("ADJUST_FUNDS"));
            engine.RegisterAction(new AdjustCurrencyAction("ADJUST_REPUTATION"));
            engine.RegisterAction(new AdjustCurrencyAction("ADJUST_SCIENCE"));
            engine.RegisterAction(new EnqueueSceneAction());
            engine.RegisterAction(new NotifyAction());
            engine.RegisterAction(new ClearNotificationAction());
            engine.RegisterAction(new PublishEventAction());
        }

        private void LoadContent()
        {
            var scenario = CampaignKitScenario.Instance;

            foreach (var triggerCfg in GameDatabase.Instance.GetConfigNodes("CAMPAIGN_TRIGGER"))
            {
                var trigger = TriggerLoader.Load(new ConfigNodeAdapter(triggerCfg));
                if (trigger != null) engine.Register(trigger);
            }

            var chapterCfgs = GameDatabase.Instance.GetConfigNodes("CAMPAIGN_CHAPTER");
            var chapterNodes = new List<ISceneNode>(chapterCfgs.Length);
            foreach (var chapterCfg in chapterCfgs)
                chapterNodes.Add(new ConfigNodeAdapter(chapterCfg));
            ChapterRegistration.RegisterAll(chapterNodes, scenario.Chapters);

            var incomeNodes = GameDatabase.Instance.GetConfigNodes("REPUTATION_INCOME");
            if (incomeNodes.Length > 0)
                scenario.Reputation.Income = ReputationLoader.LoadIncome(new ConfigNodeAdapter(incomeNodes[0]));

            var decayNodes = GameDatabase.Instance.GetConfigNodes("REPUTATION_DECAY");
            if (decayNodes.Length > 0)
                scenario.Reputation.Decay = ReputationLoader.LoadDecay(new ConfigNodeAdapter(decayNodes[0]));
        }

        private void FireFacilityForScene(GameScenes scene)
        {
            switch (scene)
            {
                case GameScenes.SPACECENTER: CampaignKit.FireFacilityEntered("SpaceCenter"); break;
                case GameScenes.EDITOR: CampaignKit.FireFacilityEntered("VehicleAssemblyBuilding"); break;
                case GameScenes.TRACKSTATION: CampaignKit.FireFacilityEntered("TrackingStation"); break;
            }
        }

        private void OnDialogueChoiceMade(string sceneId, string choiceId, string value)
        {
            sceneSource?.FireChoiceMade(sceneId, choiceId, value);
        }

        private void OnDialogueSceneEnded(string sceneId)
        {
            sceneSource?.FireSceneEnded(sceneId);
        }

        private void PlayPendingForScene(GameScenes scene)
        {
            var scenario = CampaignKitScenario.Instance;
            if (scenario == null) return;
            var facility = FacilityNameForCurrentScene();
            if (string.IsNullOrEmpty(facility)) return;
            foreach (var ps in scenario.PendingScenes.TakeForFacility(facility))
                DialogueKit.EnqueueById(ps.SceneId);
        }

        private static string FacilityNameForCurrentScene()
        {
            switch (HighLogic.LoadedScene)
            {
                case GameScenes.EDITOR: return "VehicleAssemblyBuilding";
                case GameScenes.TRACKSTATION: return "TrackingStation";
                default: return null;
            }
        }

        private void FixedUpdate()
        {
            if (engine == null) return;
            var now = Planetarium.GetUniversalTime();
            ctx.NowSeconds = now;

            if (now - lastTickSeconds > 10)
            {
                CampaignKitScenario.Instance.Reputation.Tick(ctx.Currencies, now);
                lastTickSeconds = now;
            }
        }
    }

    internal sealed class KspCurrencyAdapter : ICurrencyAdapter
    {
        public double Funds => Funding.Instance != null ? Funding.Instance.Funds : 0;
        public double Reputation => global::Reputation.Instance != null ? global::Reputation.Instance.reputation : 0;
        public double Science => ResearchAndDevelopment.Instance != null ? ResearchAndDevelopment.Instance.Science : 0;

        public void AddFunds(double amount) =>
            Funding.Instance?.AddFunds(amount, TransactionReasons.None);
        public void AddReputation(double amount) =>
            global::Reputation.Instance?.AddReputation((float)amount, TransactionReasons.None);
        public void AddScience(double amount) =>
            ResearchAndDevelopment.Instance?.AddScience((float)amount, TransactionReasons.None);
    }

    internal sealed class KspSceneEnqueuer : ISceneEnqueuer
    {
        public bool EnqueueById(string sceneId) => DialogueKit.EnqueueById(sceneId);
    }
}
