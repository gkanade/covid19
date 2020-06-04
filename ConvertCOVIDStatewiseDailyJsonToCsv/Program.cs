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
    class SimpleMovingAverage
    {
        // queue used to store list so that we get the average 
        private Queue<Double> Dataset = new Queue<Double>();
        private int period;
        private double sum;

        // constructor to initialize period 
        public SimpleMovingAverage(int period)
        {
            this.period = period;
        }

        // function to add new data in the 
        // list and update the sum so that 
        // we get the new mean 
        public void addData(double num)
        {
            sum += num;
            Dataset.Enqueue(num);

            // Updating size so that length 
            // of data set should be equal 
            // to period as a normal mean has 
            if (Dataset.Count > period)
            {
                sum -= Dataset.Dequeue();
            }
        }

        // function to calculate mean 
        public double getMean()
        {
            return sum / period;
        }
    }
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
            IDictionary<string, List<double>> dictWeeklyAvgNew = new Dictionary<string, List<double>>();
            IDictionary<string, SimpleMovingAverage> dictSMA = new Dictionary<string, SimpleMovingAverage>();

            List<string> states = new List<string>{ "an", "ap", "ar", "as",
            "br", "ch", "ct", "dd", "dl", "dn", "ga", "gj",
            "hp", "hr", "jh", "jk", "ka", "kl", "la", "ld",
            "mh", "ml", "mn", "mp", "mz", "nl", "or", "pb",
            "py", "rj", "sk", "tg", "tn", "tr", "up", "ut", "wb"};

            List<string> districts = new List<string> { "Ahmedabad", "Chennai", "Unknown", "Indore", "Jaipur", "Kolkata", "Mumbai", "Pune", "Thane"};
            List<string> statesWithTopDist = new List<string> { "Gujarat", "Tamil Nadu", "West Bengal", "Delhi", "Madhya Pradesh", "Maharashtra", "Rajasthan" };
            foreach(string state in states)
            {
                dictFatality.Add(state, new List<int>());
                dictConfirmed.Add(state, new List<int>());
                dictWeeklyAvgNew.Add(state, new List<double>());
                dictSMA.Add(state, new SimpleMovingAverage(7));
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
                        if (dictFatality[state].Count() == 0)
                        {
                            dictFatality[state].Add(currentEntry);
                        }
                        else
                        {
                            dictFatality[state].Add(currentEntry + int.Parse(dictFatality[state].Last().ToString()));
                        }
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
                        else
                        {
                            dictConfirmed[state].Add(currentEntry + int.Parse(dictConfirmed[state].Last().ToString()));
                        }

                        dictSMA[state].addData(currentEntry);
                        dictWeeklyAvgNew[state].Add(dictSMA[state].getMean());
                    }

                }
            }

            for (int i = 0; i < 11; i++) {
                dictFatalityDist["Ahmedabad"].Add(0);
                dictConfirmedDist["Ahmedabad"].Add(0);
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
                            dictConfirmedDist[district].Add(int.Parse(e["confirmed"].ToString()));
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
            foreach (string key in dictWeeklyAvgNew.Keys)
            {
                string line = "";
                Console.Write(key + " ");
                line = line + (key + ",");
                foreach (double i in dictWeeklyAvgNew[key])
                {
                    Console.Write(i + ",");
                    line = line + (i + ",");
                }
                Console.WriteLine();
                //lines.Add("");
                lines.Add(line);
            }

            using (StreamWriter op = new StreamWriter("states_data_weekly_avg_new" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".csv"))
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

            lines = new List<string>();
            foreach (string key in dictConfirmedDist.Keys)
            {
                string line = "";
                Console.Write(key + " ");
                line = line + (key + ",");
                foreach (int i in dictConfirmedDist[key])
                {
                    Console.Write(i + ",");
                    line = line + (i + ",");
                }
                Console.WriteLine();
                //lines.Add("");
                lines.Add(line);
            }

            using (StreamWriter op = new StreamWriter("districs_data_confirmed" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".csv"))
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
