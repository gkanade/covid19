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

        public static readonly List<string> AllIndiaTopDistricts = new List<string> { "Ahmedabad", "Chennai", "Unknown", "Indore", "Jaipur", "Kolkata", "Mumbai", "Pune", "Thane" };
        public static readonly List<string> AllIndiaTopStates = new List<string> { "Gujarat", "Tamil Nadu", "West Bengal", "Delhi", "Madhya Pradesh", "Maharashtra", "Rajasthan" };

        public static readonly List<string> MHAllDistricts = new List<string> { "Mumbai", "Thane", "Pune", "Aurangabad", "Nashik", "Raigad", "Palghar", "Solapur", "Jalgaon", "Akola", "Nagpur", "Kolhapur",
        "Satara", "Ratnagiri", "Amravati", "Hingoli", "Dhule", "Ahmednagar", "Jalna", "Nanded", "Yavatmal", "Sangli", "Latur", "Osmanabad", "Sindhudurg", "Buldhana", "Parbhani", "Gondia", "Beed",
        "Gadchiroli", "Bhandara", "Chandrapur", "Wardha", "Washim"};
        
    }
}
