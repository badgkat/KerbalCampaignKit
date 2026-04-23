namespace KerbalCampaignKit.Triggers.Actions
{
    public sealed class ClearNotificationAction : IAction
    {
        public string Kind => "CLEAR_NOTIFICATION";

        public void Execute(ActionSpec spec, ActionContext ctx)
        {
            var target = spec.Get("target");
            if (string.IsNullOrEmpty(target)) return;
            ctx.Notifications.Clear(target, spec.Get("source"));
        }
    }
}
