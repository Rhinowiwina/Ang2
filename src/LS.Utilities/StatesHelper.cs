using System.Collections.Generic;

namespace LS.Utilities
{
    public static class StatesHelper
    {
        public static readonly string CaliforniaStateCode = "CA";
        public static readonly string TexasStateCode = "TX";

        public static List<string> ValidStateCodes = new List<string>
        {
            "AK",
            "AZ",
            "AR",
            "CA",
            "CO",
            "CT",
            "DE",
            "FL",
            "GA",
            "HI",
            "ID",
            "IL",
            "IN",
            "IA",
            "KS",
            "KY",
            "LA",
            "ME",
            "MD",
            "MA",
            "MI",
            "MN",
            "MS",
            "MO",
            "MT",
            "NE",
            "NV",
            "NH",
            "NJ",
            "NM",
            "NY",
            "NC",
            "ND",
            "OH",
            "OK",
            "OR",
            "PA",
            "RI",
            "SC",
            "SD",
            "TN",
            "TX",
            "UT",
            "VT",
            "VA",
            "WA",
            "WV",
            "WI",
            "WY",
            "PR"
        };

        public static bool IsValidStateCode(this string stateCode)
        {
            return ValidStateCodes.Contains(stateCode);
        }
    }

}