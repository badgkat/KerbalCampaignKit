namespace KerbalCampaignKit.PendingScenes
{
    public sealed class PendingScene
    {
        public string SceneId;
        public string Facility;        // null = immediate
        public string FromTriggerId;
    }
}
