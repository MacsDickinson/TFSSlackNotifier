using System;
using System.Diagnostics;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.VersionControl.Server;

namespace TFSSlackNotifier.Notifications
{
    using System.Configuration;

    public class CheckinNotificationSlackNotifier : ISubscriber
    {
        public EventNotificationStatus ProcessEvent(TeamFoundationRequestContext requestContext,
            NotificationType notificationType,
            object notificationEventArgs,
            out int statusCode,
            out string statusMessage,
            out ExceptionPropertyCollection properties)
        {
            statusCode = 0;
            properties = null;
            statusMessage = String.Empty;

            try
            {
                LoggerHelper.LogEventTriggered(notificationEventArgs);

                var slackUpdate = new SlackUpdate
                {
                    channel = ConfigurationManager.AppSettings["Slack_Channel"],
                    username = "TFS",
                    icon_emoji = ConfigurationManager.AppSettings["Slack_Emoji"],
                    text = "This was raised in TFS"
                };
                //slackUpdate.text = string.Format("Work item {0} was updated by {1}", ev.WorkItemTitle, ev.Subscriber);

                SlackHelper.PostToSlack(slackUpdate);
            }
            catch (Exception exception)
            {
                LoggerHelper.LogEventError(notificationEventArgs, exception);
            }
            return EventNotificationStatus.ActionPermitted;
        }

        public string Name
        {
            get { return "Checkin Slack Notifier"; }
        }

        public SubscriberPriority Priority
        {
            get { return SubscriberPriority.Normal; }
        }


        public Type[] SubscribedTypes()
        {
            return new Type[] { typeof(CheckinNotification) };
        }
    }
}