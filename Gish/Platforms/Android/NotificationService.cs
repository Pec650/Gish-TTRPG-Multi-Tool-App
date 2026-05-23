using System;
using Plugin.LocalNotification;
using Plugin.LocalNotification.Core.Models;
using Plugin.LocalNotification.Core.Models.AndroidOption;

namespace Gish.Services;

public static class NotificationService
{
    public static void ScheduleSessionNotification(string sessionId, string title, string campaign, DateTime notifyDateTime)
    {
        int notificationId = Math.Abs(sessionId.GetHashCode());

        CancelSessionNotification(sessionId);

        if (notifyDateTime <= DateTime.Now)
            return;

        var request = new NotificationRequest
        {
            NotificationId = notificationId,
            Title = $"⚔️ Upcoming Session: {title}",
            Description = $"Your game for '{campaign}' starts soon! Gather your dice.",
            BadgeNumber = 1,
            Schedule = new NotificationRequestSchedule
            {
                NotifyTime = notifyDateTime,
                RepeatType = NotificationRepeat.No
            },
            Android = new AndroidOptions
            {
                VisibilityType = AndroidVisibilityType.Public,
                LaunchAppWhenTapped = true
            }
        };

        LocalNotificationCenter.Current.Show(request);
    }

    public static void CancelSessionNotification(string sessionId)
    {
        int notificationId = Math.Abs(sessionId.GetHashCode());
        LocalNotificationCenter.Current.Cancel(notificationId);
    }
}