using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

using Microsoft.Bot.Connector;
using Microsoft.Bot.Connector.Teams;
using Microsoft.Bot.Connector.Teams.Models;

using SentimentSample;

namespace Microsoft.Teams.Samples.HelloWorld.Web.Controllers
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        static Random random = new Random();

        static ScoreTracker tracker = new ScoreTracker(10); // keeps track of the last 10 messages

        [HttpPost]
        public async Task<HttpResponseMessage> Post([FromBody] Activity activity)
        {
            using (var connector = new ConnectorClient(new Uri(activity.ServiceUrl)))
            {
                if (activity.IsComposeExtensionQuery())
                {
                    var response = MessageExtension.HandleMessageExtensionQuery(connector, activity);
                    return response != null
                        ? Request.CreateResponse<ComposeExtensionResponse>(response)
                        : new HttpResponseMessage(HttpStatusCode.OK);
                }
                else if (activity.GetTextWithoutMentions().ToLower().Trim() == "score")
                {
                    await EchoBot.EchoMessage(connector, activity, tracker.GetScore());
                    return new HttpResponseMessage(HttpStatusCode.Accepted);
                }
                else
                {
                    tracker.update(TempSentiment(activity.GetTextWithoutMentions()));
                    return new HttpResponseMessage(HttpStatusCode.OK);
                }
            }
        }

        public double TempSentiment(string message)
        {
            return SentimentSample.SentimentAnalysis.CalculateSentiment(message);
        }
    }

    class ScoreTracker
    {
        double [] recentScores;
        int index;
        double totalScore;
        int size;
        int total_size;

        public ScoreTracker(int total_size)
        {
            recentScores = new Double[total_size];
            index = 0;
            totalScore = 0.0;
            size = 0;
            this.total_size = total_size;
        }

        public void update(double nscore)
        {
            recentScores[index] = nscore;
            index++;

            if (this.size < this.total_size) this.size++;
            var b = 0;
        }

        public double GetScore()
        {
            var coeff = 1.5;
            var score = 0.0;
            var totalWeight = 0.0;
            for (int i = 0; i < this.size; i++)
            {
                int j = (index - 1 - i) % this.size;
                score += recentScores[j] * coeff;
                totalWeight += coeff;
                coeff /= 1.5;
            }
            if (totalWeight == 0) return 0.5;
            return score / totalWeight;
        }
    }
}
