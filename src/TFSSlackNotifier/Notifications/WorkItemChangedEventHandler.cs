using System;
using System.Diagnostics;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server;

namespace TFSSlackNotifier.Notifications
{
    using System.Configuration;

    public class WorkItemChangedEventHandler : ISubscriber
    {
        public Type[] SubscribedTypes()
        {
            return new[] { typeof(WorkItemChangedEvent) };
        }

        public string Name
        {
            get { return "WorkItemChangedEventHandler"; }
        }

        public SubscriberPriority Priority
        {
            get { return SubscriberPriority.Normal; }
        }

        public EventNotificationStatus ProcessEvent(TeamFoundationRequestContext requestContext, NotificationType notificationType,
                                                    object notificationEventArgs, out int statusCode, out string statusMessage,
                                                    out ExceptionPropertyCollection properties)
        {
            statusCode = 0;
            properties = null;
            statusMessage = String.Empty;
            const string cs = "TFS Services";

            if (!EventLog.SourceExists(cs))
                EventLog.CreateEventSource(cs, "Application");

            try
            {
                LoggerHelper.LogEventTriggered(notificationEventArgs);

                EventLog.WriteEntry(cs, string.Format("{0} Triggered", notificationEventArgs.GetType()), EventLogEntryType.Warning);

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
    }
}
