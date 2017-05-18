using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS.ApiBindingModels {
  public  class PaymentViewBindingModel {
       public decimal? TotalAmount { get; set; }
        public string TransactionID { get; set; }
        public string ProcessID { get; set; }
        public DateTime? DatePaid { get; set; }
        public int? NumberOfPayments { get; set; }
        }
    public class PaymentDetailViewBindingModel {
        public string Email { get; set; }
        public int? NumberOfPayments { get; set; }
        public decimal? TotalAmount { get; set; }
        }
    }
