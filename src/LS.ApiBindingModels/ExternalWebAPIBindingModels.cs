using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS.ApiBindingModels {
    public class UpdateOrderStatusRequest {
        public string Id {get; set;}
        public bool Approved { get; set; }
        public string RTR_Name { get; set; }
        public DateTime RTR_Date { get; set; }
        public string RTR_Notes { get; set; }
        public string RTR_RejectCode { get; set; }
    }
    public class UpdateOrderStatusGetOrderRequest {
        public string Id { get; set; }
        public string StatusID { get; set; }
    }

    public class UpdateExternalUserIDsRequest {
        public string ExternalUserID { get; set; }
        public string Status { get; set; }
    }
}
