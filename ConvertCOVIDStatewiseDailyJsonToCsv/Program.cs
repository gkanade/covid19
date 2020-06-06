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
            DistrictsScenario allIndiaTop = new DistrictsScenario { name = "AllIndiaTop", listDistricts = Constants.AllIndiaTopDistricts, listStates = Constants.AllIndiaTopStates };
            DistrictsScenario maharashtra = new DistrictsScenario { name = "Maharashtra", listDistricts = Constants.MHAllDistricts, listStates = new List<string> { "Maharashtra" } };
            districts.Scenario(allIndiaTop);
            districts.Scenario(maharashtra);
            States.ComputeStatesData();
        
        }
    }
}
