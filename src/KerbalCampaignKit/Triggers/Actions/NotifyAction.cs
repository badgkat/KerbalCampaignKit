using System;
using KerbalCampaignKit.Notifications;

namespace KerbalCampaignKit.Triggers.Actions
{
    public sealed class NotifyAction : IAction
    {
        public string Kind => "NOTIFY";

        public void Execute(ActionSpec spec, ActionContext ctx)
        {
            var target = spec.Get("target");
            if (string.IsNullOrEmpty(target)) return;

            var sevText = spec.Get("severity") ?? "Info";
            if (!Enum.TryParse<NotificationSeverity>(sevText, out var severity))
                severity = NotificationSeverity.Info;

            var clearOnText = spec.Get("clearOn") ?? "Manual";
            if (!Enum.TryParse<NotificationClearOn>(clearOnText, out var clearOn))
                clearOn = NotificationClearOn.Manual;

            ctx.Notifications.Add(new Notification
            {
                Target = target,
                Severity = severity,
                Source = spec.Get("source") ?? string.Empty,
                ClearOn = clearOn,
                ClearSceneId = spec.Get("clearSceneId"),
                ClearFlag = spec.Get("clearFlag"),
                ClearFlagValue = spec.Get("clearFlagValue"),
                AddedAtSeconds = ctx.NowSeconds,
            });
        }
    }
}
