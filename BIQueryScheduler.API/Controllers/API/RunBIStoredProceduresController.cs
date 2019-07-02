using BIQueryScheduler.API.Models;
using BIQueryScheduler.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;

namespace BIQueryScheduler.API.Controllers.API
{
    [Route("BIQueryScheduler")]
    public class RunBIStoredProceduresController : ControllerBase
    {
        private readonly ILogger Logger = Log.ForContext<RunBIStoredProceduresController>();

        public RunBIStoredProceduresController(IConfiguration configuration, IRunBIStoredProceduresService runBIStoredProceduresService) : base()
        {
            Configuration = configuration;
            RunBIStoredProceduresService = runBIStoredProceduresService;
        }

        private IRunBIStoredProceduresService RunBIStoredProceduresService;
        private readonly IConfiguration Configuration;

        /// <summary>
        /// Run BI Stored Procedures
        /// </summary>
        /// <returns>HTTP Status Code and possibly a JSON body</returns>
        [HttpPost]
        [Route("RunBIStoredProcedures")]
        public IActionResult RunBIStoredProcedures()
        {
            try
            {
                var biConfiguration = new BIDataBase();
                Configuration.GetSection("BIDataBase").Bind(biConfiguration);
                var res = RunBIStoredProceduresService.RunBIStoredProcedures(biConfiguration);
                return Ok();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ex.Message);
                return BadRequest(ex);
            }
        }
    }
}
