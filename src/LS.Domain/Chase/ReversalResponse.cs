using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS.Domain.Chase {
    public class ReversalResponse {
        public ErrorMessage errorMessage { get; set; }
        public string approvalStatus { get; set; }
        public string bin { get; set; }
        public string hostRespCode { get; set; }
        public string lastRetryDate { get; set; }
        public string merchantID { get; set; }
        public string orderID { get; set; }
        public string outstandingAmt { get; set; }
        public string procStatus { get; set; }
        public string procStatusMessage { get; set; }
        public string respCode { get; set; }
        public string respDateTime { get; set; }
        public string retryAttempCount { get; set; }
        public string retryTrace { get; set; }
        public string terminalID { get; set; }
        public string txRefIdx { get; set; }
        public string txRefNum { get; set; }
        public string version { get; set; }
    }
}
