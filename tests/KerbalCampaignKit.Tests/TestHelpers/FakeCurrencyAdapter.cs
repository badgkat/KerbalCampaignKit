using KerbalCampaignKit.Triggers.Actions;

namespace KerbalCampaignKit.Tests.TestHelpers
{
    public sealed class FakeCurrencyAdapter : ICurrencyAdapter
    {
        public double Funds { get; private set; }
        public double Reputation { get; private set; }
        public double Science { get; private set; }

        public void AddFunds(double amount) { Funds += amount; }
        public void AddReputation(double amount) { Reputation += amount; }
        public void AddScience(double amount) { Science += amount; }
    }
}
