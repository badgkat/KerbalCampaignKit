using KerbalDialogueKit.Flags;

namespace KerbalCampaignKit.Triggers.Events
{
    /// <summary>
    /// Publishes contract status as KDK flags so trigger whenExpression
    /// and CC FlagEquals/FlagExpression requirements can query contract state directly.
    /// Flag format: contract:&lt;name&gt;.&lt;state&gt; = "true"
    /// where state is one of accepted | complete | failed | cancelled.
    /// </summary>
    public static class ContractStateFlagPublisher
    {
        public static void Publish(FlagStore flags, string contractName, string state)
        {
            if (flags == null) return;
            if (string.IsNullOrEmpty(contractName)) return;
            if (string.IsNullOrEmpty(state)) return;
            flags.Set($"contract:{contractName}.{state}", "true");
        }
    }
}
