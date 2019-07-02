using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using BIQueryScheduler.API.Services.Interfaces;
using Serilog;
using BIQueryScheduler.API.Models;

namespace BIQueryScheduler.API.Services
{
    public class RunBIStoredProceduresService : IRunBIStoredProceduresService
    {
        private static readonly ILogger Logger = Log.ForContext<RunBIStoredProceduresService>();

        public async Task<bool> RunBIStoredProcedures(BIDataBase biDataBase)
        {
            try
            {
                Task procedureOne = ExecuteStoredProceduresNonQuery(biDataBase.StoredProcedures.SP1, biDataBase.ConnectionString);
                Task procedureTwo = ExecuteStoredProceduresNonQuery(biDataBase.StoredProcedures.SP2, biDataBase.ConnectionString);
                Task procedureThree = ExecuteStoredProceduresNonQuery(biDataBase.StoredProcedures.SP3, biDataBase.ConnectionString);
                Task procedureFour = ExecuteStoredProceduresNonQuery(biDataBase.StoredProcedures.SP4, biDataBase.ConnectionString);
                Task procedureFive = ExecuteStoredProceduresNonQuery(biDataBase.StoredProcedures.SP5, biDataBase.ConnectionString);
                Task procedureSix = ExecuteStoredProceduresNonQuery(biDataBase.StoredProcedures.SP6, biDataBase.ConnectionString);
                Task procedureSeven = ExecuteStoredProceduresNonQuery(biDataBase.StoredProcedures.SP7, biDataBase.ConnectionString);
                Task procedureEight = ExecuteStoredProceduresNonQuery(biDataBase.StoredProcedures.SP8, biDataBase.ConnectionString);
                Task procedureNine = ExecuteStoredProceduresNonQuery(biDataBase.StoredProcedures.SP9, biDataBase.ConnectionString);

                await Task.WhenAll(procedureOne, procedureTwo, procedureThree, procedureFour, procedureFive, procedureSix, procedureSeven, procedureEight, procedureNine);
                return true;
            }
            catch (Exception ex)
            {
                Log.Error($"Error during the execution of BI stored procedures: {ex.Message}");
                throw ex;
            }
        }

        public async Task ExecuteStoredProceduresNonQuery(string sqlCommandText, string ConnectionString)
        {
            using (var sqlConnection = new SqlConnection(ConnectionString))
            {
                sqlConnection.Open();
                using (var sqlTransaction = sqlConnection.BeginTransaction())
                {
                    try
                    {
                        using (var sqlCommand = new SqlCommand { CommandText = sqlCommandText, CommandType = CommandType.StoredProcedure, Connection = sqlConnection })
                        {
                            sqlCommand.Transaction = sqlTransaction;
                            await sqlCommand.ExecuteNonQueryAsync();
                        }
                    }
                    catch (Exception e)
                    {
                        sqlTransaction.Rollback();
                        throw new Exception(e.Message);
                    }
                    sqlTransaction.Commit();
                }
            }
        }
    }
}
