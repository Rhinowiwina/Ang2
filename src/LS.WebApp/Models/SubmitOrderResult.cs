using System.Collections.Generic;

namespace LS.WebApp.Models
{
    public class SubmitOrderResult
    {
        public SubmitOrderResult()
        {
            Errors = new List<string>();
        }

        public List<string> Errors { get; set; }
        public bool TpivBypassAvailable { get; set; }
        public bool AddressBypassAvailable { get; set; }
        public string TpivBypassFailureType { get; set; }
        public string AssignedPhoneNumber { get; set; }
    }
}