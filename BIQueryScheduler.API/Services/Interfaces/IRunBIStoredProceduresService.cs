using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BIQueryScheduler.API.Models;

namespace BIQueryScheduler.API.Services.Interfaces
{
    public interface IRunBIStoredProceduresService
    {
        Task<bool> RunBIStoredProcedures(BIDataBase storedProcedures);

        Task ExecuteStoredProceduresNonQuery(string sqlCommandText, string ConnectionString);
    }
}
