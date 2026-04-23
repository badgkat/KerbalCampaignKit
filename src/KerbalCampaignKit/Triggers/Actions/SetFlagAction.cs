namespace KerbalCampaignKit.Triggers.Actions
{
    public sealed class SetFlagAction : IAction
    {
        public string Kind => "SET_FLAG";

        public void Execute(ActionSpec spec, ActionContext ctx)
        {
            var name = spec.Get("name");
            var value = spec.Get("value");
            if (string.IsNullOrEmpty(name)) return;
            ctx.Flags.Set(name, value ?? string.Empty);
        }
    }
}
