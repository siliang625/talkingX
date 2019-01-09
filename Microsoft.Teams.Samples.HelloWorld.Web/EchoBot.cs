using System.Threading.Tasks;

using Microsoft.Bot.Connector;
using Microsoft.Bot.Connector.Teams;

namespace Microsoft.Teams.Samples.HelloWorld.Web
{
    public class EchoBot
    {
        public static async Task EchoMessage(ConnectorClient connector, Activity activity)
        {
            var responseString = "not a question!";
            if (activity.GetTextWithoutMentions() == "")
                responseString = QuestionHandler("what is money?");

            var reply = activity.CreateReply(responseString);
            await connector.Conversations.ReplyToActivityWithRetriesAsync(reply);
        }

        public static string QuestionHandler(string questionText)
        {
            // search knowledge
            return "";
        }
    }
}
