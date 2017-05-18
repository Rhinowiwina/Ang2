using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS.Domain.Chase {
    public class ReversalRequest {
        public string txRefNum { get; set; }
        public string txRefIdx { get; set; }
        public string orderID { get; set; }
    }
}
