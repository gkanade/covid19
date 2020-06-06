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
        static string sourceFile = "https://api.covid19india.org/districts_daily.json";
        static string rawJsonFile = "districts_daily.json";
        static JObject rawJson;
        protected static List<string> districts;
        protected static List<string> states;

        protected static IDictionary<string, List<int>> dictFatality = new Dictionary<string, List<int>>();
        protected static IDictionary<string, List<int>> dictConfirmed = new Dictionary<string, List<int>>();
        
        public Districts()
        {
            using (var client = new WebClient())
            {
                client.DownloadFile(sourceFile, rawJsonFile);
            }

            rawJson = JObject.Parse(File.ReadAllText(rawJsonFile));
            
        }

        public void Scenario(List<string> dist, List<string> stat)
        {
            districts = dist;
            states = stat;
            ComputeStatistics();
        }

        void InitializeStats()
        {
            foreach (string district in districts)
            {
                dictFatality.Add(district, new List<int>());
                dictConfirmed.Add(district, new List<int>());
            }
        }

        void ReadInStats()
        {
            foreach (string state in states)
            {
                var entry = rawJson["districtsDaily"][state];
                foreach (string district in districts)
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
                        }
                    }
                }

            }
        }

        private void WriteFiles(CaseType caseType)
        {
            List<string> lines = new List<string>();
            Dictionary<string, List<int>> dict = caseType == CaseType.Confirmed ? 
                (Dictionary<string, List<int>>)dictConfirmed : (Dictionary<string, List<int>>)dictFatality;
            
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

            using (StreamWriter op = new StreamWriter("districs_data_" + caseType.ToString().ToLower() + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".csv"))
            {
                foreach (string line in lines)
                {
                    op.WriteLine(line);
                }
                op.Flush();
            }

        }

        void WriteFiles()
        {
            WriteFiles(CaseType.Confirmed);
            WriteFiles(CaseType.Fatality);
        }

        void ComputeStatistics()
        {
            InitializeStats();
            for (int i = 0; i < 11; i++)
            {
                dictFatality["Ahmedabad"].Add(0);
                dictConfirmed["Ahmedabad"].Add(0);
            }
            ReadInStats();
            WriteFiles();
        }
    }
}
