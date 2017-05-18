using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using LS.Core;
using LS.Core.Interfaces;
using LS.Utilities;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using LS.ApiBindingModels;
namespace LS.Services {
    public class SolixAPIDataService {
        protected ILog Logger { get; set; }
        private static string _ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public SolixAPIDataService() {
            Logger = LoggerFactory.GetLogger(GetType());
        }

    }
}
