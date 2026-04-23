using System;

namespace KerbalCampaignKit.Triggers.Events
{
    public interface IEventSource
    {
        void Register(Action<EventRecord> onEvent);
        void Unregister();
    }
}
