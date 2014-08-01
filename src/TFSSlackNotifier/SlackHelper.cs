using System.Net.Http;
using System.Text;
using System.Web.Script.Serialization;

namespace TFSSlackNotifier
{
    using System.Configuration;

    internal static class SlackHelper
    {
        private static string ApiKey
        {
            get { return ConfigurationManager.AppSettings["Slack_Key"]; }
        }
        private static string Project
        {
            get { return ConfigurationManager.AppSettings["Slack_Project"]; }
        }

        public static async void PostToSlack(SlackUpdate slackUpdate)
        {
            var client = new HttpClient();

            var jsonString = new JavaScriptSerializer().Serialize(slackUpdate);

            // Get the response.
            await client.PostAsync(string.Format("https://{0}.slack.com/services/hooks/incoming-webhook?token={1}", Project, ApiKey),
                                    new StringContent(jsonString, Encoding.UTF8, "application/json"));
        }
    }

    public class SlackUpdate
    {
        public string channel { get; set; }
        public string username { get; set; }
        public string icon_emoji { get; set; }
        public string text { get; set; }
    }
}
