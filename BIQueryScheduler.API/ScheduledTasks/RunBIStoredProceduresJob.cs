using BIQueryScheduler.API.Models;
using BIQueryScheduler.API.Services.Interfaces;
using Microsoft.Extensions.Options;
using Quartz;
using Serilog;
using System.Threading.Tasks;

namespace BIQueryScheduler.API.ScheduledTasks
{
    /// <summary>
    /// IJob interface to be implemented by this job class to have executed by the scheduler.
    /// </summary>
    public class RunBIStoredProceduresJob : IJob
    {
        private static readonly ILogger Logger = Log.ForContext<RunBIStoredProceduresJob>();

        private readonly IRunBIStoredProceduresService RunBIStoredProceduresService;
        public BIDataBase BIDataBase;

        public RunBIStoredProceduresJob(IRunBIStoredProceduresService runBIStoredProceduresService, IOptions<BIDataBase> biDataBase)
        {
            RunBIStoredProceduresService = runBIStoredProceduresService;
            BIDataBase = biDataBase.Value;
        }


        /// <summary>
        /// When job's trigger is fired this execute method is invoked
        /// </summary>
        /// <param name="context">This parameter contains info about the job's runtime</param>
        public Task Execute(IJobExecutionContext context)
        {
            Logger.Debug("Started RunBIStoredProcedures ");
            var result = RunBIStoredProceduresService.RunBIStoredProcedures(BIDataBase);
            Logger.Debug("Completed RunBIStoredProcedures successfully");
            return result;
        }
    }
}
