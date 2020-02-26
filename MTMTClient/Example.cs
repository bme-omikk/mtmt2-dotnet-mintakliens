using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MTMTClient
{
    class Example
    {                
        static async Task Main(string[] args)
        {
            using (MTMTAPIService mtmtAPIService = new MTMTAPIService(new Uri("https://oktatas.mtmt.hu/"), "hkonrad", "namilegyenezajelszo"))
            {
                //get authors with given name István and print out their family names
                JObject authors = await mtmtAPIService.Get("api/author", new Parameters("cond", "givenName;eq;István"));
                JArray jArray = (JArray)authors["content"];
                foreach (var o in jArray)
                {
                    Console.WriteLine((string)o["familyName"]);
                }
                //Get publication with id 109815 and print the json out
                JObject pub = await mtmtAPIService.Get($"api/publication/109815");
                Console.WriteLine(pub.ToString());
            }
        }
    }
}
