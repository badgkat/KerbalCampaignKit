namespace KerbalCampaignKit.Reputation
{
    public sealed class ReputationDecay
    {
        public bool Enabled;
        public double RatePercentPerMonth = 1.5;
        public bool ResetOnContractComplete = true;
        public bool TierFloors = true;
        public double HaltUntilSeconds;

        public bool IsHalted(double nowSeconds) => nowSeconds < HaltUntilSeconds;

        public double ComputeDecay(double currentRep, double elapsedDays, double floor)
        {
            if (!Enabled) return 0;
            if (currentRep <= floor) return 0;

            var monthlyDecay = currentRep * (RatePercentPerMonth / 100.0);
            var decay = monthlyDecay * (elapsedDays / 30.0);

            var maxDecay = currentRep - floor;
            if (decay > maxDecay) decay = maxDecay;
            if (decay < 0) decay = 0;
            return decay;
        }
    }
}
