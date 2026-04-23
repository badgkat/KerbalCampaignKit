using KerbalCampaignKit.Chapters;
using KerbalDialogueKit.Flags;

namespace KerbalCampaignKit.Core
{
    public sealed class StateMirror
    {
        private readonly FlagStore flags;
        private readonly ChapterManager chapters;

        public StateMirror(FlagStore flags, ChapterManager chapters)
        {
            this.flags = flags;
            this.chapters = chapters;
            chapters.OnChapterChanged += (_, to) => flags.Set("chapter", to ?? string.Empty);
        }

        public void SyncAll()
        {
            flags.Set("chapter", chapters.Current ?? string.Empty);
            if (global::Reputation.Instance != null)
                flags.Set("reputation", ((int)global::Reputation.Instance.reputation).ToString());
            if (Funding.Instance != null)
                flags.Set("funds", ((long)Funding.Instance.Funds).ToString());
            if (ResearchAndDevelopment.Instance != null)
                flags.Set("science", ((int)ResearchAndDevelopment.Instance.Science).ToString());

            SyncFacilityLevel("Administration");
            SyncFacilityLevel("MissionControl");
            SyncFacilityLevel("TrackingStation");
            SyncFacilityLevel("ResearchAndDevelopment");
            SyncFacilityLevel("VehicleAssemblyBuilding");
            SyncFacilityLevel("SpaceplaneHangar");
            SyncFacilityLevel("AstronautComplex");
            SyncFacilityLevel("LaunchPad");
            SyncFacilityLevel("Runway");
        }

        private void SyncFacilityLevel(string facilityName)
        {
            var level = (int)(ScenarioUpgradeableFacilities.GetFacilityLevel(
                $"SpaceCenter/{facilityName}") * 2) + 1;
            flags.Set($"facility.{facilityName}", level.ToString());
        }
    }
}
