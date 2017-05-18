using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LS.Core;
using LS.Core.Interfaces;

namespace LS.ApiBindingModels
{
    public class AccountBalanceBindingModel
    {
        public bool IsError { get; set; }
        public string PlanVoice { get; set; }
        public string PlanText { get; set; }
        public string PlanMMS { get; set; }
        public string PlanCombo { get; set; }
        public string PlanData { get; set; }
        public string PlanExpire { get; set; }
        public string TopUpVoice { get; set; }
        public string TopUpText { get; set; }
        public string TopUpMMS { get; set; }
        public string TopUpCombo { get; set; }
        public string TopUpData { get; set; }
        public string TopUpExpire { get; set; }
        public bool TopUpExpirationSet { get; set; }
        public string ServiceEndDate { get; set; }
    }
  
   

}
