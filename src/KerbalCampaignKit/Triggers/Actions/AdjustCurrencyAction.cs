namespace KerbalCampaignKit.Triggers.Actions
{
    public sealed class AdjustCurrencyAction : IAction
    {
        private readonly string kind;

        public AdjustCurrencyAction(string kind) { this.kind = kind; }
        public string Kind => kind;

        public void Execute(ActionSpec spec, ActionContext ctx)
        {
            var amountText = spec.Get("amount");
            if (string.IsNullOrEmpty(amountText)) return;
            if (!double.TryParse(amountText, out var amount)) return;

            switch (kind)
            {
                case "ADJUST_FUNDS": ctx.Currencies.AddFunds(amount); break;
                case "ADJUST_REPUTATION": ctx.Currencies.AddReputation(amount); break;
                case "ADJUST_SCIENCE": ctx.Currencies.AddScience(amount); break;
            }
        }
    }
}
