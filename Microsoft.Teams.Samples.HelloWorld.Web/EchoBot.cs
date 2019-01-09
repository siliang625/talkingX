using System.Threading.Tasks;

using Microsoft.Bot.Connector;
using Microsoft.Bot.Connector.Teams;

namespace Microsoft.Teams.Samples.HelloWorld.Web
{
    public class EchoBot
    {
        public static async Task EchoMessage(ConnectorClient connector, Activity activity)
        {
            var reply = activity.CreateReply("I don't understand you.");
            await connector.Conversations.ReplyToActivityWithRetriesAsync(reply);
        }
    }
}
