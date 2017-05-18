using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLClient
{
    public class SQLQueryProperties {
        private static string _ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public SQLQueryProperties() {
            ConnectionString = _ConnectionString;
            Timeout = 20;
            CommandParameters = new List<SqlParameter>();
            CommandType = CommandType.Text;
            ReturnList = false;

        }

        public string ConnectionString { get; set; }
        public string CommandText { get; set; }
        public int Timeout { get; set; }
        public CommandType CommandType { get; set; }
        public List<SqlParameter> CommandParameters {get;set;}
        public bool ReturnList { get; set; }
        public bool ReturnSqlIdentityId { get; set; }
    }

    public class SQLParam {
        public string Name { get; set; }
        public object Value { get; set; }
    }
}
