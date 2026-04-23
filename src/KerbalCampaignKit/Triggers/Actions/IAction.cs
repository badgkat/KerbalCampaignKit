namespace KerbalCampaignKit.Triggers.Actions
{
    public interface IAction
    {
        string Kind { get; }
        void Execute(ActionSpec spec, ActionContext ctx);
    }
}
