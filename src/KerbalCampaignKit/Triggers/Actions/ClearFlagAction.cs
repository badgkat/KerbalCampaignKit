namespace KerbalCampaignKit.Triggers.Actions
{
    public sealed class ClearFlagAction : IAction
    {
        public string Kind => "CLEAR_FLAG";

        public void Execute(ActionSpec spec, ActionContext ctx)
        {
            var name = spec.Get("name");
            if (string.IsNullOrEmpty(name)) return;
            ctx.Flags.Remove(name);
        }
    }
}
