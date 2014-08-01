using System;
using System.Diagnostics;
using Microsoft.TeamFoundation.Framework.Server;

namespace TFSSlackNotifier
{
    public static class LoggerHelper
    {
        const string Source = "TFS Services";

        private static void CreateSource()
        {
            if (!EventLog.SourceExists(Source))
                EventLog.CreateEventSource(Source, "Application");
        }

        public static void LogEventTriggered(object notificationEventArgs)
        {
            CreateSource();
            EventLog.WriteEntry(Source, string.Format("{0} Triggered", notificationEventArgs.GetType()), EventLogEntryType.Warning);
        }

        public static void LogEventError(object notificationEventArgs, Exception exception)
        {
            CreateSource();
            TeamFoundationApplicationCore.LogException("Error processing event", exception);
            EventLog.WriteEntry(Source, "Error processing WorkItemChangedEventHandler event", EventLogEntryType.Error);
        }
    }
}
