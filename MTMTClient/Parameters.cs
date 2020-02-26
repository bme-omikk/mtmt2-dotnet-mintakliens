using System;
using System.Collections.Generic;
using System.Text;

namespace MTMTClient
{
    public class Parameters
    {
        Dictionary<string, string> keyValuePairs;
        public Parameters()
        {
            keyValuePairs = new Dictionary<string, string>();
        }
        public Parameters(params string[] list)
        {
            keyValuePairs = new Dictionary<string, string>();
            for (int i = 0; i < list.Length; i+=2)
            {
                keyValuePairs.Add(list[i], list[i + 1]);
            }
        }
        public void Add(string key, string value)
        {
            keyValuePairs.Add(key, value);
        }
        public string ToQuery()
        {
            string Result = "?";
            foreach (KeyValuePair<string, string> part in keyValuePairs)
            {
                Result += $"{part.Key}={part.Value}&";
            }
            Result = Result.Substring(0, Result.Length - 1);
            return Result;
        }
    }
}
