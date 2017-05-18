using LS.Core.Interfaces;

namespace LS.Domain
{
    public class ChangeESN_VerizonDetails : IChangeESN_VerizonDetails
    {
        public string MDN { get; set; }
        public string BudgetMobileID { get; set; }
        public string DeviceID { get; set; }
        public string Order_ID { get; set; }
    }
}
