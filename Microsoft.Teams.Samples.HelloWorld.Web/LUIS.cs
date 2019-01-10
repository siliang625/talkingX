using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Microsoft.Teams.Samples.HelloWorld.Web
{
    public class LUIS
    {
        public string query { get; set; }
        // public IList<Intent> intents { get; set; }
        //public IList<Entity> entities { get; set; }
    }

    public class TopScoringIntent
    {
        public string intent { get; set; }
        public double score { get; set; }
    }

    public class Entity
    {
        public string entity { get; set; }
        public string type { get; set; }
        public int startIndex { get; set; }
        public int endIndex { get; set; }
        public object resolution { get; set; }
        public double score { get; set; }
    }

    public class LUISRequest
    {
        public string query { get; set; }
        public TopScoringIntent topScoringIntent { get; set; }
        public List<Entity> entities { get; set; }
    }


    public class Intent
    {
        public string intent { get; set; }
        public double score { get; set; }
    }

    public class SentimentAnalysis
    {
        public string label { get; set; }
        public double score { get; set; }
    }

    public class LUISResult
    {
        public string query { get; set; }
        public TopScoringIntent topScoringIntent { get; set; }
        public List<Intent> intents { get; set; }
        public List<Entity> entities { get; set; }
        public SentimentAnalysis sentimentAnalysis { get; set; }
    }

}