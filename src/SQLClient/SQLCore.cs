using Core;
using Exceptionless;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace SQLClient {
    public static class SQLCore {
        public static ApiProcessingResult<int> ExecuteNonQuery(SQLQueryProperties _sqlProperties) {
            var result = new ApiProcessingResult<int>();

            using (var connection = new SqlConnection(_sqlProperties.ConnectionString)) {
                using (var command = new SqlCommand(_sqlProperties.CommandText, connection)) {
                    try {
                        command.CommandType = _sqlProperties.CommandType;
                        command.Parameters.Clear();
                        if (_sqlProperties.CommandParameters != null) { command.Parameters.AddRange(_sqlProperties.CommandParameters.ToArray()); }
                        connection.Open();

                        var sqlResult = command.ExecuteNonQuery();
                        result.Data = sqlResult;
                    } catch (Exception ex) {
                        result.IsError = true;

                        result.Errors.Add(new ApiProcessingError("Error processing ExecuteNonQueryAsync", "Error processing ExecuteNonQueryAsync", ""));
                    } finally {
                        connection.Close();
                    }

                    return result;
                }
            }
        }

        public static ApiProcessingResult<object> ExecuteReader<T>(SQLQueryProperties _sqlProperties) {
            var processingResult = new ApiProcessingResult<object>();

            using (var connection = new SqlConnection(_sqlProperties.ConnectionString)) {
                using (var command = new SqlCommand(_sqlProperties.CommandText, connection)) {
                    try {
                        var dt = new DataTable();
                        command.CommandType = _sqlProperties.CommandType;
                        command.Parameters.Clear();
                        if (_sqlProperties.CommandParameters != null) { command.Parameters.AddRange(_sqlProperties.CommandParameters.ToArray()); }
                        command.CommandTimeout = _sqlProperties.Timeout;
                        connection.Open();

                        using (SqlDataAdapter a = new SqlDataAdapter(command)) {
                            a.Fill(dt);
                        }

                        if (dt.Rows.Count < 1) {
                            processingResult.Data =null;
                            return processingResult;
                        }

                        try {
                            IList<T> ValsList;
                            T ValsObject;

                            if (_sqlProperties.ReturnList) {
                                ValsList = CollectionHelper.ConvertTo<T>(dt);
                                processingResult.Data = ValsList;
                            } else {
                                ValsObject = CollectionHelper.ConvertToObject<T>(dt);
                                processingResult.Data = ValsObject;
                            }
                        } catch (Exception ex) {
                            processingResult.IsError = true;
                            processingResult.Errors.Add(new ApiProcessingError("Error retrieving data from ExecuteReader.", "Error retrieving data from ExecuteReader.", ""));

                            ex.ToExceptionless()
                               .SetMessage("Collection Helper Error")
                               .AddTags("Collection Helper")
                               .MarkAsCritical()
                               .Submit();
                        }

                        return processingResult;
                    } catch (Exception ex) {
                        processingResult.IsError = true;

                        ex.ToExceptionless()
                           .SetMessage("Error running ExecuteReaderAsync(object).")
                           .AddTags("Sql Query Wrapper")
                           .AddObject(_sqlProperties.CommandText, "Query")
                           .AddObject(Utils.CommandParametersToObj(_sqlProperties.CommandParameters.ToArray()), "Query Parameters")
                           .MarkAsCritical()
                           .Submit();
                        processingResult.Errors.Add(new ApiProcessingError("Error running ExecuteReaderAsync. Ex: " + ex.ToString(), "Error running ExecuteReaderAsync. Ex: " + ex.ToString(), ""));
                        return processingResult;
                    } finally {
                        connection.Close();
                    }
                }
            }
        }
        public static ApiProcessingResult<object> ExecuteReader(SQLQueryProperties _sqlProperties) {
            var processingResult = new ApiProcessingResult<object>();
            //returns table
            using (var connection = new SqlConnection(_sqlProperties.ConnectionString)) {
                using (var command = new SqlCommand(_sqlProperties.CommandText,connection)) {
                    try {
                        var dt = new DataTable();
                        command.CommandType = _sqlProperties.CommandType;
                        command.Parameters.Clear();
                        if (_sqlProperties.CommandParameters != null) { command.Parameters.AddRange(_sqlProperties.CommandParameters.ToArray()); }
                        command.CommandTimeout = _sqlProperties.Timeout;
                        connection.Open();

                        using (SqlDataAdapter a = new SqlDataAdapter(command)) {
                            a.Fill(dt);
                            }

                        if (dt.Rows.Count < 1) {
                            processingResult.Data = null;

                            }else {
                            processingResult.Data = dt;

                            }
                           return processingResult;

                        
                  
                        } catch (Exception ex) {
                        processingResult.IsError = true;

                        ex.ToExceptionless()
                           .SetMessage("Error running ExecuteReaderAsync(object).")
                           .AddTags("Sql Query Wrapper")
                           .AddObject(_sqlProperties.CommandText,"Query")
                           .AddObject(Utils.CommandParametersToObj(_sqlProperties.CommandParameters.ToArray()),"Query Parameters")
                           .MarkAsCritical()
                           .Submit();
                        processingResult.Errors.Add(new ApiProcessingError("Error running ExecuteReaderAsync. Ex: " + ex.ToString(),"Error running ExecuteReaderAsync. Ex: " + ex.ToString(),""));
                        return processingResult;
                        } finally {
                        connection.Close();
                        }
                    }
                }
            }
        public static ApiProcessingResult<string> ExecuteScalar(SQLQueryProperties _sqlProperties) {
            var result = new ApiProcessingResult<string>();
            if (_sqlProperties.ReturnSqlIdentityId) {
                 _sqlProperties.CommandText += ";SELECT CAST(scope_identity() AS varchar)";
                }
            
            using (var connection = new SqlConnection(_sqlProperties.ConnectionString)) {
                using (var command = new SqlCommand(_sqlProperties.CommandText, connection)) {
                    try {
                        command.CommandType = _sqlProperties.CommandType;
                        command.Parameters.Clear();
                        
                        if (_sqlProperties.CommandParameters != null) { command.Parameters.AddRange(_sqlProperties.CommandParameters.ToArray()); }
                        connection.Open();

						var sqlResult = command.ExecuteScalar();
						if (sqlResult == null)
						{
							sqlResult = "";
						}
                        result.Data =sqlResult.ToString();
                    } catch (Exception ex) {
                        result.IsError = true;

                        ex.ToExceptionless()
                          .SetMessage("Error running ExecuteScalarAsync.")
                          .AddTags("SqlQueryWrapper")
                          .AddObject(_sqlProperties.CommandText, "Query")
                          .AddObject(Utils.CommandParametersToObj(_sqlProperties.CommandParameters.ToArray()), "Query Parameters")
                          .MarkAsCritical()
                          .Submit();
                        result.Errors.Add(new ApiProcessingError("Error processing ExecuteScalarAsync Ex: " + ex.ToString(), "Error processing ExecuteScalarAsync Ex: " + ex.ToString(), ""));
                    } finally {
                        connection.Close();
                    }

                    return result;
                }
            }
        }

    }
}
