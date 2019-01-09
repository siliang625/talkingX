using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Http;
using System.Net.Http.Headers;



namespace Microsoft.Teams.Samples.HelloWorld.Web.External
{
    public class DataObject
    {
        public string Name { get; set; }
    }
    public class BingEntitySearch
    {
        private const string URL = "https://api.cognitive.microsoft.com/bing/v7.0/entities";
        public static string search(string query)
        {
            return "";
        }
    }
}