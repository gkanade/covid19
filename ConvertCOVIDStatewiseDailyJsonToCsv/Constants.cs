using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvertCOVIDStatewiseDailyJsonToCsv
{
    class Constants
    {
        public enum CaseType { Fatality, Confirmed};

        public static readonly List<string> allIndiaDistricts = new List<string> { "Ahmedabad", "Chennai", "Unknown", "Indore", "Jaipur", "Kolkata", "Mumbai", "Pune", "Thane" };
        public static readonly List<string> statesWithTopDistricts = new List<string> { "Gujarat", "Tamil Nadu", "West Bengal", "Delhi", "Madhya Pradesh", "Maharashtra", "Rajasthan" };
        
    }
}
