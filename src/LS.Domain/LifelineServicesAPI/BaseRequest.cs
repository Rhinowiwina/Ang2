using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS.Domain.LifelineServicesAPI {
    public class BaseRequest {
        public string APIKey { get; set; }
        public string CompanyID { get; set;}
        public string SalesTeamID { get; set; }
    }
}
