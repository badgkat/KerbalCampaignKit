namespace KerbalCampaignKit.Triggers.Actions
{
    public sealed class AdvanceChapterAction : IAction
    {
        public string Kind => "ADVANCE_CHAPTER";

        public void Execute(ActionSpec spec, ActionContext ctx)
        {
            var target = spec.Get("target");
            if (string.IsNullOrEmpty(target)) return;
            ctx.Chapters.Advance(target, ctx.NowSeconds);
        }
    }
}
