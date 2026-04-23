using KerbalDialogueKit.Flags;

namespace KerbalCampaignKit.Triggers
{
    public static class RequirementExpression
    {
        public static bool Evaluate(string expression, FlagStore flags)
        {
            if (string.IsNullOrEmpty(expression)) return true;

            FlagExpression parsed;
            try
            {
                parsed = FlagExpressionParser.Parse(expression);
            }
            catch
            {
                return false;
            }
            if (parsed == null) return false;
            return parsed.Evaluate(flags);
        }
    }
}
