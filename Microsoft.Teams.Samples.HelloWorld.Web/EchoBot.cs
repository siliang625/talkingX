using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Connector.Teams;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Teams.Samples.HelloWorld.Web.Controllers;

namespace Microsoft.Teams.Samples.HelloWorld.Web
{
    public class EchoBot
    {
        public static async Task EchoMessage(ConnectorClient connector, Activity activity, double score, ScoreTracker tracker)
        {
            var responseString = "your score is: " + score.ToString();

            var intentString = activity.GetTextWithoutMentions();

            var command = (intentString.IndexOf(" ") > -1) ? intentString.Substring(0, intentString.IndexOf(" ")) : intentString;

            switch (command)
            {
                case "Entity":
                    string host = "https://api.cognitive.microsoft.com";
                    string path = "/bing/v7.0/entities";
                    string market = "en-US";               
                    string key = "50f20daf6202450a82824126b0a5afb4";
                    string query = intentString.Substring(7);
                    HttpClient client = new HttpClient();
                    client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", key);
                    string uri = host + path + "?mkt=" + market + "&q=" + System.Net.WebUtility.UrlEncode(query);
                    HttpResponseMessage response = await client.GetAsync(uri);
                    string responsible = await response.Content.ReadAsStringAsync();
                    JObject parsed = JObject.Parse(responsible);
                    await connector.Conversations.ReplyToActivityWithRetriesAsync(activity.CreateReply(parsed["entities"]["value"][0]["description"].ToString()));
                    break;
                default:
                    if (intentString != null)
                    {
                        LUISResult result = await MakeRequest(intentString);
                        switch (result.intents[0].intent)
                        {
                            case "AskedForName":
                                if (score >= 0.7)
                                    responseString = "hi friend!!!! my name is dev-X";
                                else if (score >= 0.5 && score < 0.7)
                                    responseString = "my name is dev-x";
                                else if (score > 0.3 && score < 0.5)
                                    responseString = "my name is dev-X, are u alright buddy?";
                                else
                                    responseString = "my name is dev-x, you look very sad, let me tell you a joke!" + "\n" + TellJoke();

                                break;

                            case "Born":
                                if (score > 0.6)
                                    responseString = "Vancouver BC (during aloha hacks)";
                                else
                                    responseString = "Vancouver BC (During aloha hacks) :D";
                                break;
                            default:
                                tracker.update(TempSentiment(activity.GetTextWithoutMentions()));
                                score = tracker.GetScore();
                                if (score > 0.3 && score < 0.5)
                                    responseString = "Are u alright buddy?";
                                else if (score <= 0.3)
                                    responseString = "You recent activities make me believe you are not in a good mood, let me tell you a joke!" + "\n" + TellJoke();
                                else
                                    responseString = "Well, I have absorbed all you just said!";
                                break;

                        }
                    }

                    var reply = activity.CreateReply(responseString);
                    await connector.Conversations.ReplyToActivityWithRetriesAsync(reply);
                    break;
            }
        }

        public static double TempSentiment(string message)
        {
            return SentimentSample.SentimentAnalysis.CalculateSentiment(message);
        }
        public static async Task SendMessage(ConnectorClient connector, Activity activity, string message)
        {
            var reply = activity.CreateReply(message);
            await connector.Conversations.ReplyToActivityWithRetriesAsync(reply);
        }

        public static string QuestionHandler(string questionText)
        {
            // search knowledge
            return "";
        }

        static async Task<LUISResult> MakeRequest(string Query)
        {
            var client = new HttpClient();
            var queryString = System.Web.HttpUtility.ParseQueryString(Query);

            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", WebConfigurationManager.AppSettings["LUIS_Subscription_Key"]);

            var uri = WebConfigurationManager.AppSettings["LUIS_Url"] + queryString;

            HttpResponseMessage response;

            // Request body
            byte[] byteData = Encoding.UTF8.GetBytes("{body}");

            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                response = await client.GetAsync(uri);
                string responseBody = await response.Content.ReadAsStringAsync();
                LUISResult lResult = JsonConvert.DeserializeObject<LUISResult>(responseBody);
                return lResult;
            }
        }

        public static string TellJoke()
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri("https://icanhazdadjoke.com/"),
                Method = HttpMethod.Get,
            };
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            JObject parsedJoke = new JObject();
            var task = client.SendAsync(request)
                .ContinueWith((taskwithmsg) =>
                {
                    var response = taskwithmsg.Result;

                    var jsonTask = response.Content.ReadAsStringAsync();
                    jsonTask.Wait();
                    parsedJoke = JObject.Parse(jsonTask.Result);
                });
            task.Wait();

            string status = parsedJoke["status"].ToString();
            string actualJoke = parsedJoke["joke"].ToString();
            if (status == "200")
            {
                return actualJoke;
            }
            else
            {
                return "I am sorry, I am out of jokes";
            }

        }
    }
}
