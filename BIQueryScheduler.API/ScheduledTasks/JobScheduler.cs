using BIQueryScheduler.API.Models;
using BIQueryScheduler.API.Services;
using BIQueryScheduler.API.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Impl;
using System.Collections.Specialized;
using System.Threading.Tasks;

namespace BIQueryScheduler.API.ScheduledTasks
{
    public class JobScheduler
    {
        static async Task<IScheduler> GetScheduler()
        {
            NameValueCollection props = new NameValueCollection
            {
                { "quartz.serializer.type", "binary" }
            };
            var factory = new StdSchedulerFactory(props);
            return await factory.GetScheduler();
        }
        public class DemoJobOptions
        {
            public string Url { get; set; }
        }
        /// <summary>
        /// Define Quartz job scheduler's executing using CronTrigger.
        /// Refer this url for cron trigger expressions
        /// https://www.quartz-scheduler.net/documentation/quartz-2.x/tutorial/crontriggers.html
        /// </summary>
        public static void ConfigureScheduler(IConfiguration configuration)
        {
            string cronSchedule = configuration["QuartzJobTime"] as string;
            var services = new ServiceCollection();
            // Register job
            services.AddTransient<RunBIStoredProceduresJob>();

            // Register job dependencies
            services.AddTransient<IRunBIStoredProceduresService, RunBIStoredProceduresService>();
            services.AddOptions();
            services.Configure<BIDataBase>(
            options => configuration.GetSection("BIDataBase").Bind(options));
            var container = services.BuildServiceProvider();

            // Create an instance of the job factory
            var jobFactory = new JobFactory(container);

            Task<IScheduler> scheduler = GetScheduler();
            scheduler.Wait();
            IScheduler Scheduler = scheduler.Result;
            Scheduler.JobFactory = jobFactory;
            Scheduler.Start();

            var runBIStoredProceduresJob = JobBuilder.Create<RunBIStoredProceduresJob>()
                .WithIdentity("RunBIStoredProcedures")
                .Build();

            var runBIStoredProceduresTrigger = TriggerBuilder.Create()
                .WithIdentity("RunBIStoredProceduresCron")
                .StartNow()
                .WithCronSchedule(cronSchedule)
                .Build();

            Scheduler.ScheduleJob(runBIStoredProceduresJob, runBIStoredProceduresTrigger).Wait();
        }
    }
}
