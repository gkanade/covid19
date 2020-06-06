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
    class Districts
    {
        static string sourceFile = "https://api.covid19india.org/districts_daily.json";
        static string rawJsonFile = "districts_daily.json";
        static List<string> districts = new List<string> { "Ahmedabad", "Chennai", "Unknown", "Indore", "Jaipur", "Kolkata", "Mumbai", "Pune", "Thane" };
        static List<string> statesWithTopDist = new List<string> { "Gujarat", "Tamil Nadu", "West Bengal", "Delhi", "Madhya Pradesh", "Maharashtra", "Rajasthan" };


        Districts()
        {
            using (var client = new WebClient())
            {
                client.DownloadFile(sourceFile, rawJsonFile);
            }
        }

        public static void ComputeDistrictsData()
        {            
            JObject o2 = JObject.Parse(File.ReadAllText(rawJsonFile));
            IDictionary<string, List<int>> dictFatalityDist = new Dictionary<string, List<int>>();
            IDictionary<string, List<int>> dictConfirmedDist = new Dictionary<string, List<int>>();
            
            foreach (string district in districts)
            {
                dictFatalityDist.Add(district, new List<int>());
                dictConfirmedDist.Add(district, new List<int>());
            }

            for (int i = 0; i < 11; i++)
            {
                dictFatalityDist["Ahmedabad"].Add(0);
                dictConfirmedDist["Ahmedabad"].Add(0);
            }
            foreach (string state in statesWithTopDist)
            {
                var entry = o2["districtsDaily"][state];
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
                            dictFatalityDist[district].Add(int.Parse(e["deceased"].ToString()));
                            dictConfirmedDist[district].Add(int.Parse(e["confirmed"].ToString()));
                        }
                    }
                }

            }

            List<string> lines = new List<string>();
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
