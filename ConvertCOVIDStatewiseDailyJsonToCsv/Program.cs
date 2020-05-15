using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ConvertCOVIDStatewiseDailyJsonToCsv
{
    class Program
    {
        static void Main(string[] args)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
        | SecurityProtocolType.Tls11
        | SecurityProtocolType.Tls12
        | SecurityProtocolType.Ssl3;
            using (var client = new WebClient())
            {
                client.DownloadFile("https://api.covid19india.org/states_daily.json", "states_daily.json");
                client.DownloadFile("https://api.covid19india.org/districts_daily.json", "districts_daily.json");
            }
            JObject o1 = JObject.Parse(File.ReadAllText("states_daily.json"));
            JObject o2 = JObject.Parse(File.ReadAllText("districts_daily.json"));
            IDictionary<string, List<int>> dictFatality = new Dictionary<string, List<int>>();
            IDictionary<string, List<int>> dictConfirmed = new Dictionary<string, List<int>>();
            IDictionary<string, List<int>> dictFatalityDist = new Dictionary<string, List<int>>();
            IDictionary<string, List<int>> dictConfirmedDist = new Dictionary<string, List<int>>();

            List<string> states = new List<string>{ "an", "ap", "ar", "as",
            "br", "ch", "ct", "dd", "dl", "dn", "ga", "gj",
            "hp", "hr", "jh", "jk", "ka", "kl", "la", "ld",
            "mh", "ml", "mn", "mp", "mz", "nl", "or", "pb",
            "py", "rj", "sk", "tg", "tn", "tr", "up", "ut", "wb"};

            List<string> districts = new List<string> { "Ahmedabad", "Chennai", "Unknown", "Kolkata", "Mumbai", "Pune"};
            List<string> statesWithTopDist = new List<string> { "Gujarat", "Tamil Nadu", "West Bengal", "Delhi", "Maharashtra" };
            foreach(string state in states)
            {
                dictFatality.Add(state, new List<int>());
                dictConfirmed.Add(state, new List<int>());
            }

            foreach (string district in districts)
            {
                dictFatalityDist.Add(district, new List<int>());
                dictConfirmedDist.Add(district, new List<int>());
            }

            int numEntries = o1["states_daily"].Count();
            for(int i = 0; i < numEntries; i++)
            {
                var entry = o1["states_daily"][i];
                if(entry["status"].ToString().Equals("Deceased"))
                {
                    foreach(string state in states)
                    {
                        int currentEntry = entry[state].ToString() == "" ? 0 : int.Parse(entry[state].ToString());
                        if(dictFatality[state].Count() == 0)
                        {
                            dictFatality[state].Add(currentEntry);
                        }
                        dictFatality[state].Add(currentEntry + int.Parse(dictFatality[state].Last().ToString()));
                    }
                    
                }
                if (entry["status"].ToString().Equals("Confirmed"))
                {
                    foreach (string state in states)
                    {
                        int currentEntry = entry[state].ToString() == "" ? 0 : int.Parse(entry[state].ToString());
                        if (dictConfirmed[state].Count() == 0)
                        {
                            dictConfirmed[state].Add(currentEntry);
                        }
                        dictConfirmed[state].Add(currentEntry + int.Parse(dictConfirmed[state].Last().ToString()));
                    }

                }
            }

            for (int i = 0; i < 11; i++) {
                dictFatalityDist["Ahmedabad"].Add(0);
                    }
            foreach(string state in statesWithTopDist)
            {
                var entry = o2["districtsDaily"][state];
                foreach(string district in districts)
                {
                    if(entry[district] != null )
                    {
                        if(district == "Unknown" && state != "Delhi")
                        {
                            continue;
                        }
                        int numDays = entry[district].Count();
                        for(int i = 0; i < numDays; i++)
                        {
                            var e = entry[district][i];
                            dictFatalityDist[district].Add(int.Parse(e["deceased"].ToString()));
                        }
                    }
                }

            }
            
            List<string> lines = new List<string>();
            foreach(string key in dictFatality.Keys)
            {
                string line = "";
                Console.Write(key + " ");
                line=line+(key + ",");
                foreach(int i in dictFatality[key])
                {
                    Console.Write(i + ",");
                    line=line+(i + ",");
                }
                Console.WriteLine();
                //lines.Add("");
                lines.Add(line);
            }

            using (StreamWriter op = new StreamWriter("states_data_deaths"+ DateTime.Now.ToString("yyyyMMddHHmmssfff")+".csv"))
            {
                foreach (string line in lines)
                {
                    op.WriteLine(line);
                }
                op.Flush();
            }
            lines = new List<string>();
            foreach (string key in dictConfirmed.Keys)
            {
                string line = "";
                Console.Write(key + " ");
                line = line + (key + ",");
                foreach (int i in dictConfirmed[key])
                {
                    Console.Write(i + ",");
                    line = line + (i + ",");
                }
                Console.WriteLine();
                //lines.Add("");
                lines.Add(line);
            }

            using (StreamWriter op = new StreamWriter("states_data_confirmed"+ DateTime.Now.ToString("yyyyMMddHHmmssfff")+".csv"))
            {
                foreach (string line in lines)
                {
                    op.WriteLine(line);
                }
                op.Flush();
            }




            lines = new List<string>();
            foreach (string key in dictFatalityDist.Keys)
            {
                string line = "";
                Console.Write(key + " ");
                line = line + (key + ",");
                foreach (int i in dictFatalityDist[key])
                {
                    Console.Write(i + ",");
                    line = line + (i + ",");
                }
                Console.WriteLine();
                //lines.Add("");
                lines.Add(line);
            }

            using (StreamWriter op = new StreamWriter("districs_data_fatality" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".csv"))
            {
                foreach (string line in lines)
                {
                    op.WriteLine(line);
                }
                op.Flush();
            }
        }
    }
}
