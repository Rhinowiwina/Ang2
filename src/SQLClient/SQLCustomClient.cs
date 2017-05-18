using LS.Core;
using Exceptionless;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LS.Utilities;

namespace SQLClient {
    public class SQLCustomClient {

        public SQLCustomClient() {
            Target = new SQLQueryProperties();
        }

        public SQLQueryProperties Target { get; private set; }

        // Functions
        public ServiceProcessingResult<object> Select<T>() {return SQLCore.ExecuteReader<T>(Target);}
        public ServiceProcessingResult<object> SelectMany<T>() {Target.ReturnList = true;return SQLCore.ExecuteReader<T>(Target);}
        public ServiceProcessingResult<object> SelectMany() { Target.ReturnList = false; return SQLCore.ExecuteReader(Target); }
        public ServiceProcessingResult<int> Update() {return SQLCore.ExecuteNonQuery(Target);}
        public ServiceProcessingResult<string> Insert() {
            return SQLCore.ExecuteScalar(Target); }

        //Setters
        public SQLCustomClient ConnectionString(string ConnectionString) {Target.ConnectionString = ConnectionString;return this;}
        public SQLCustomClient Timeout(int Timeout) {Target.Timeout = Timeout;return this;}
        public SQLCustomClient CommandType(CommandType CommandType) {Target.CommandType = CommandType; return this;}
        public SQLCustomClient CommandText(string CommandText) {Target.CommandText = CommandText;return this;}
        public SQLCustomClient AddParameter(string name, object value) {Target.CommandParameters.Add(new SqlParameter(name, value));return this;}
        public SQLCustomClient AddParameter(string name, ICollection<string> list) {
            DataTable LocList = new DataTable();
            LocList.Columns.Add("Item", typeof(String));
            if (list.Count() > 0) {
                foreach (string Id in list) {
                    LocList.Rows.Add(Id);
                }
            } else {
                LocList.Rows.Add("");
            }

            Target.CommandParameters.Add(new SqlParameter(name, SqlDbType.Structured) { TypeName = "ItemList", Value = LocList });
            return this;
        }
		public SQLCustomClient ClearParameters()
		{
			Target.CommandParameters.Clear();
			return this;

		}
        public SQLCustomClient ReturnList(bool returnList) {Target.ReturnList = returnList;return this;}
        public SQLCustomClient ReturnSqlIndentityId(bool val) { Target.ReturnSqlIdentityId = val; return this; }
        }
}
