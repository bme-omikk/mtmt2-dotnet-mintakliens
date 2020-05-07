using System;
using System.Collections.Generic;
using System.Text;

namespace MTMTClient
{
    public class Parameters
    {
        List<(string, string)> Pairs;
        public Parameters()
        {
            Pairs = new List<(string, string)>();
        }
        public Parameters(params string[] list)
        {
            Pairs = new List<(string, string)>();
            for (int i = 0; i < list.Length; i += 2)
            {
                Pairs.Add((list[i], list[i + 1]));
            }
        }
        public void Add(string key, string value)
        {
            Pairs.Add((key, value));
        }
        public string ToQuery()
        {
            string Result = "?";
            foreach (var pair in Pairs)
            {
                Result += $"{pair.Item1}={pair.Item2}&";
            }
            Result = Result.Substring(0, Result.Length - 1);
            return Result;
        }
    }
}
