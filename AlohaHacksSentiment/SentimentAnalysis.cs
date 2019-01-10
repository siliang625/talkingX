using System;
using System.Net.Http;
using System.Text;

namespace SentimentSample
{
    class SentimentAnalysis
    {
        static string host = "https://westus.api.cognitive.microsoft.com";
        static string path = "/text/analytics/v2.0/sentiment";

        // NOTE: Replace this example key with a valid subscription key.
        static string key = "d2aeb2e7963a48c79d6b5e677c339935";

        async static void Search(string sentence)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", key);

            string uri = host + path;

            string sentence_json = ConvertToJson(sentence);

            HttpResponseMessage response = await client.PostAsync(uri,
                new StringContent(sentence_json, Encoding.UTF8, "application/json"));

            string contentString = await response.Content.ReadAsStringAsync();
            Console.WriteLine(JsonPrettyPrint(contentString));
        }

        static string ConvertToJson(string sent)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{\n\t\"documents\": [");
            sb.Append("\n\t\t{");
            sb.Append("\n\t\t\t\"language\": \"en\",");
            sb.Append("\n\t\t\t\"id\": \"1\",");
            sb.Append("\n\t\t\t\"text\": \"" + sent + "\"");
            sb.Append("\n\t\t}");
            sb.Append("\n\t]\n}");
            return sb.ToString();
        }

        static void Main(string[] args)
        {
            Search("I love this city");
            Console.ReadLine();
        }


        static string JsonPrettyPrint(string json)
        {
            if (string.IsNullOrEmpty(json))
                return string.Empty;

            json = json.Replace(Environment.NewLine, "").Replace("\t", "");

            StringBuilder sb = new StringBuilder();
            bool quote = false;
            bool ignore = false;
            int offset = 0;
            int indentLength = 3;

            foreach (char ch in json)
            {
                switch (ch)
                {
                    case '"':
                        if (!ignore) quote = !quote;
                        break;
                    case '\'':
                        if (quote) ignore = !ignore;
                        break;
                }

                if (quote)
                    sb.Append(ch);
                else
                {
                    switch (ch)
                    {
                        case '{':
                        case '[':
                            sb.Append(ch);
                            sb.Append(Environment.NewLine);
                            sb.Append(new string(' ', ++offset * indentLength));
                            break;
                        case '}':
                        case ']':
                            sb.Append(Environment.NewLine);
                            sb.Append(new string(' ', --offset * indentLength));
                            sb.Append(ch);
                            break;
                        case ',':
                            sb.Append(ch);
                            sb.Append(Environment.NewLine);
                            sb.Append(new string(' ', offset * indentLength));
                            break;
                        case ':':
                            sb.Append(ch);
                            sb.Append(' ');
                            break;
                        default:
                            if (ch != ' ') sb.Append(ch);
                            break;
                    }
                }
            }

            return sb.ToString().Trim();
        }

    }
}