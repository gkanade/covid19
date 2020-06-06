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
    
    class Program
    {
    

        static void Main(string[] args)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
        | SecurityProtocolType.Tls11
        | SecurityProtocolType.Tls12
        | SecurityProtocolType.Ssl3;

            Districts districts = new Districts();
            districts.Scenario(Constants.AllIndiaTopDistricts, Constants.AllIndiaTopStates);
            districts.Scenario(Constants.MHAllDistricts, new List<string> { "Maharashtra" });
            States.ComputeStatesData();
        
        }
    }
}
