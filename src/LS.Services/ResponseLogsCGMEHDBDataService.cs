using System;
using System.Threading.Tasks;
using LS.Core;
using LS.Core.Interfaces.PayPal;
using LS.Domain;
using LS.Domain.PayPal;
using LS.Repositories;
using LS.Repositories.DBContext;
using LS.Utilities;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using LS.PayPal;
using System.Data.Entity;

namespace LS.Services {
    public class ResponseLogsCGMEHDBDataService : BaseDataService<ResponseLogsCGMEHDB, string> {
        private static readonly string EmailSubject = "Sched Task: Lifeline Services Commission Payment Error";
        private static readonly string EmailToAddress = "errors@spinlifeserv.com";


        public override BaseRepository<ResponseLogsCGMEHDB, string> GetDefaultRepository() {
            return new ResponseLogsCGMEHDBRepository();
        }
        


    }
}