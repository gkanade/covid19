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
    class States
    {
        public static void ComputeStatesData()
        {
            using (var client = new WebClient())
            {
                client.DownloadFile("https://api.covid19india.org/states_daily.json", "states_daily.json");
            }
            JObject o1 = JObject.Parse(File.ReadAllText("states_daily.json"));
            IDictionary<string, List<int>> dictFatality = new Dictionary<string, List<int>>();
            IDictionary<string, List<int>> dictConfirmed = new Dictionary<string, List<int>>();
            IDictionary<string, List<double>> dictWeeklyAvgNew = new Dictionary<string, List<double>>();
            IDictionary<string, SimpleMovingAverage> dictSMA = new Dictionary<string, SimpleMovingAverage>();
            IDictionary<string, List<double>> dictWeeklyAvgNewDeaths = new Dictionary<string, List<double>>();
            IDictionary<string, SimpleMovingAverage> dictSMADeaths = new Dictionary<string, SimpleMovingAverage>();

            List<string> states = new List<string>{ "an", "ap", "ar", "as",
            "br", "ch", "ct", "dd", "dl", "dn", "ga", "gj",
            "hp", "hr", "jh", "jk", "ka", "kl", "la", "ld",
            "mh", "ml", "mn", "mp", "mz", "nl", "or", "pb",
            "py", "rj", "sk", "tg", "tn", "tr", "up", "ut", "wb"};

            foreach (string state in states)
            {
                dictFatality.Add(state, new List<int>());
                dictConfirmed.Add(state, new List<int>());
                dictWeeklyAvgNew.Add(state, new List<double>());
                dictSMA.Add(state, new SimpleMovingAverage(7));
                dictWeeklyAvgNewDeaths.Add(state, new List<double>());
                dictSMADeaths.Add(state, new SimpleMovingAverage(7));
            }

            int numEntries = o1["states_daily"].Count();
            for (int i = 0; i < numEntries; i++)
            {
                var entry = o1["states_daily"][i];
                if (entry["status"].ToString().Equals("Deceased"))
                {
                    foreach (string state in states)
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
                        dictSMADeaths[state].addData(currentEntry);
                        dictWeeklyAvgNewDeaths[state].Add(dictSMA[state].getMean());
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

            List<string> lines = new List<string>();
            foreach (string key in dictFatality.Keys)
            {
                string line = "";
                Console.Write(key + " ");
                line = line + (key + ",");
                foreach (int i in dictFatality[key])
                {
                    Console.Write(i + ",");
                    line = line + (i + ",");
                }
                Console.WriteLine();
                //lines.Add("");
                lines.Add(line);
            }

            using (StreamWriter op = new StreamWriter("states_data_deaths" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".csv"))
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

            using (StreamWriter op = new StreamWriter("states_data_confirmed" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".csv"))
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
            foreach (string key in dictWeeklyAvgNewDeaths.Keys)
            {
                string line = "";
                Console.Write(key + " ");
                line = line + (key + ",");
                foreach (double i in dictWeeklyAvgNewDeaths[key])
                {
                    Console.Write(i + ",");
                    line = line + (i + ",");
                }
                Console.WriteLine();
                //lines.Add("");
                lines.Add(line);
            }

            using (StreamWriter op = new StreamWriter("states_data_weekly_avg_new_deaths" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".csv"))
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
