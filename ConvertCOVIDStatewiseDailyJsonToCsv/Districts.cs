using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static ConvertCOVIDStatewiseDailyJsonToCsv.Constants;

namespace ConvertCOVIDStatewiseDailyJsonToCsv
{
    
    class Districts
    {
        string sourceFile = "https://api.covid19india.org/districts_daily.json";
        string rawJsonFile = "districts_daily.json";
        static JObject rawJson;
        

        
        public Districts()
        {
            using (var client = new WebClient())
            {
                client.DownloadFile(sourceFile, rawJsonFile);
            }

            rawJson = JObject.Parse(File.ReadAllText(rawJsonFile));
            
        }

        public void Scenario(DistrictsScenario scenario)
        {
            IDictionary<string, List<int>> dictFatality = new Dictionary<string, List<int>>();
            IDictionary<string, List<int>> dictConfirmed = new Dictionary<string, List<int>>();
            IDictionary<string, List<double>> dictWeeklyAvgNew = new Dictionary<string, List<double>>();
            IDictionary<string, SimpleMovingAverage> dictSMA = new Dictionary<string, SimpleMovingAverage>();
            IDictionary<string, List<double>> dictWeeklyAvgNewDeaths = new Dictionary<string, List<double>>();
            IDictionary<string, SimpleMovingAverage> dictSMADeaths = new Dictionary<string, SimpleMovingAverage>();

            foreach (string district in scenario.listDistricts)
            {
                dictFatality.Add(district, new List<int>());
                dictConfirmed.Add(district, new List<int>());
                dictWeeklyAvgNew.Add(district, new List<double>());
                dictSMA.Add(district, new SimpleMovingAverage(7));
                dictWeeklyAvgNewDeaths.Add(district, new List<double>());
                dictSMADeaths.Add(district, new SimpleMovingAverage(7));
            }

            if (scenario.listDistricts.Contains("Ahmedabad"))
            {
                for (int i = 0; i < 11; i++)
                {
                    dictFatality["Ahmedabad"].Add(0);
                    dictConfirmed["Ahmedabad"].Add(0);
                }
            }
            foreach (string state in scenario.listStates)
            {
                var entry = rawJson["districtsDaily"][state];
                foreach (string district in scenario.listDistricts)
                {
                    if (entry[district] != null)
                    {
                        if (district == "Unknown" && state != "Delhi")
                        {
                            continue;
                        }
                        int numDays = entry[district].Count();
                        for (int i = 0; i < numDays; i++)
                        {
                            var e = entry[district][i];
                            dictFatality[district].Add(int.Parse(e["deceased"].ToString()));
                            dictConfirmed[district].Add(int.Parse(e["confirmed"].ToString()));
                            if (dictSMADeaths[district].Count() == 0)
                            {
                                dictSMADeaths[district].addData(int.Parse(e["deceased"].ToString()));
                            } else
                            {
                                dictSMADeaths[district].addData(int.Parse(e["deceased"].ToString()) - dictSMADeaths[district].Last());
                            }
                            dictWeeklyAvgNewDeaths[district].Add(dictSMADeaths[district].getMean());
                            if (dictSMA[district].Count() == 0)
                            {
                                dictSMA[district].addData(int.Parse(e["deceased"].ToString()));
                            }
                            else
                            {
                                dictSMA[district].addData(int.Parse(e["deceased"].ToString()) - dictSMA[district].Last());
                            }
                            
                            dictWeeklyAvgNew[district].Add(dictSMA[district].getMean());
                        }
                    }
                }

            }
            WriteFiles(CaseType.Confirmed, scenario, dictConfirmed);
            WriteFiles(CaseType.Fatality, scenario, dictFatality);
            WriteFiles2(CaseType.Confirmed, scenario, dictWeeklyAvgNew);
            WriteFiles2(CaseType.Fatality, scenario, dictWeeklyAvgNewDeaths);
        }

        
        private void WriteFiles2(CaseType caseType, DistrictsScenario scenario, IDictionary<string, List<double>> dict)
        {
            List<string> lines = new List<string>();

            foreach (string key in dict.Keys)
            {
                string line = "";
                Console.Write(key + " ");
                line = line + (key + ",");
                foreach (int i in dict[key])
                {
                    Console.Write(i + ",");
                    line = line + (i + ",");
                }
                Console.WriteLine();
                //lines.Add("");
                lines.Add(line);
            }

            using (StreamWriter op = new StreamWriter("districs_data_" + caseType.ToString().ToLower() + "_" + scenario.name + "_weeklyavg_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".csv"))
            {
                foreach (string line in lines)
                {
                    op.WriteLine(line);
                }
                op.Flush();
            }

        }
    

        private void WriteFiles(CaseType caseType, DistrictsScenario scenario, IDictionary<string, List<int>> dict)
        {
            List<string> lines = new List<string>();
            
            foreach (string key in dict.Keys)
            {
                string line = "";
                Console.Write(key + " ");
                line = line + (key + ",");
                foreach (int i in dict[key])
                {
                    Console.Write(i + ",");
                    line = line + (i + ",");
                }
                Console.WriteLine();
                //lines.Add("");
                lines.Add(line);
            }

            using (StreamWriter op = new StreamWriter("districs_data_" + caseType.ToString().ToLower() + "_" + scenario.name + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".csv"))
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
