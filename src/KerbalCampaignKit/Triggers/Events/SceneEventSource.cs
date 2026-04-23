using System;

namespace KerbalCampaignKit.Triggers.Events
{
    public sealed class SceneEventSource : IEventSource
    {
        private Action<EventRecord> sink;

        public void Register(Action<EventRecord> onEvent) { sink = onEvent; }
        public void Unregister() { sink = null; }

        public void FireSceneEnded(string sceneId)
        {
            if (sink == null) return;
            var record = new EventRecord { Type = EventType.SceneEnded };
            record.Params["sceneId"] = sceneId;
            sink(record);
        }

        public void FireChoiceMade(string sceneId, string choiceId, string value)
        {
            if (sink == null) return;
            var record = new EventRecord { Type = EventType.SceneChoiceMade };
            record.Params["sceneId"] = sceneId;
            record.Params["choiceId"] = choiceId;
            record.Params["value"] = value;
            sink(record);
        }
    }
}
