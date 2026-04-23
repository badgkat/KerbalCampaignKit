using KerbalCampaignKit.PendingScenes;

namespace KerbalCampaignKit.Triggers.Actions
{
    public sealed class EnqueueSceneAction : IAction
    {
        public string Kind => "ENQUEUE_SCENE";

        public void Execute(ActionSpec spec, ActionContext ctx)
        {
            var sceneId = spec.Get("sceneId");
            if (string.IsNullOrEmpty(sceneId)) return;

            var when = spec.Get("when") ?? "Immediate";
            if (when == "OnFacilityEnter")
            {
                var facility = spec.Get("facility");
                if (string.IsNullOrEmpty(facility)) return;
                ctx.PendingScenes.Add(new PendingScene
                {
                    SceneId = sceneId,
                    Facility = facility
                });
            }
            else
            {
                ctx.SceneEnqueuer?.EnqueueById(sceneId);
            }
        }
    }
}
