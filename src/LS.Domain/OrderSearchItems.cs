using System;
using System.Collections.Generic;
using LS.Core.Interfaces;

namespace LS.Domain
{
    public class OrderSearhItems
    {
        public string Filter { get; set; }
      
        //comma seperated string
        public ICollection <string> SalesTeamList { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }

    }
}

