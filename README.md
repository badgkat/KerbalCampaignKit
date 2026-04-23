# KerbalCampaignKit

A utility library that extends [KerbalDialogueKit](https://github.com/badgkat/KerbalDialogueKit)
with a career-scale state machine: named narrative chapters, a general event→action
trigger system, a reputation economy, and a hierarchical notification API. Built for
KSP 1.12.x mods that want story-driven career progression.

## Status

Early development. v0.1.0.

## Features

- **Chapters** — named narrative state with entry/exit triggers, save-persisted
- **Triggers** — "when X happens, do Y" wiring with 20+ event types and 10 action types
- **Reputation economy** — opt-in tiered passive income, decay, gates, stakes
- **Notifications** — hierarchical attention markers with auto-clear rules
- **CC integration** — `InChapter`, `ChapterAtLeast`, `FlagEquals`, `FlagNotEquals`, `FlagExpression`, `ReputationMinimum` REQUIREMENTs + `ReputationStake` BEHAVIOUR

## Requirements

- KSP 1.12.x
- [KerbalDialogueKit](https://github.com/badgkat/KerbalDialogueKit) v0.1.0 or later
- [ContractConfigurator](https://forum.kerbalspaceprogram.com/index.php?/topic/91625-112x-contract-configurator/) v2.12.0 or later

## Installation

Extract so the folder structure is:

```
GameData/
  KerbalCampaignKit/
    KerbalCampaignKit.version
    Plugins/
      KerbalCampaignKit.dll
```

## Quick Example

Define a chapter and a trigger in any `.cfg` file inside `GameData`:

```cfg
CAMPAIGN_CHAPTER
{
    id = 2
    name = Into Orbit
    ENTRY_TRIGGER = chapter_2_entry
}

CAMPAIGN_TRIGGER
{
    id = chapter_2_entry
    once = true
    ON_EVENT
    {
        type = ContractComplete
        contract = BKEX_UnmannedOrbit
    }
    ACTIONS
    {
        ADVANCE_CHAPTER
        {
            target = 2
        }
        ENQUEUE_SCENE
        {
            sceneId = chapter_2_intro
            when = OnFacilityEnter
            facility = Administration
        }
    }
}
```

See `Examples/demo-campaign.cfg` for a more complete example.

## License

MIT — see `LICENSE`.
