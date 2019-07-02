using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.WindowsServices;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Quartz;
using Quartz.Impl;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.RollingFileAlternate;
using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Collections.Specialized;
using System.Threading.Tasks;
using BIQueryScheduler.API.ScheduledTasks;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace BIQueryScheduler
{
    public class Program
    {
        public const string BIQuerySchedulerURL = "http://BIQueryScheduler.io";
        public const int BIQuerySchedulerDebugPort = 4005;

        private static readonly ILogger Logger = Log.ForContext<Program>();

        private static string pathToExe;
        private static string pathToContentRoot;

        public static bool DownloadInProgress = false;
        public static int DownloadProgress = 0;
        private static IScheduler _scheduler { get; set; }

        /// <summary>
        /// Working directory the application launched from
        /// </summary>
        public static string WorkingDirectory => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        /// <summary>
        /// .NET Configuration Service
        /// </summary>
        public static IConfiguration Configuration => new ConfigurationBuilder()
                .SetBasePath(WorkingDirectory)
                .AddJsonFile("appSettings.json", false, true)
                .AddEnvironmentVariables()
                .Build();

        public static void Main(string[] args)
        {
            pathToExe = Assembly.GetExecutingAssembly().Location;
            pathToContentRoot = Path.GetDirectoryName(pathToExe);

            ConfigureLogging();

            JobScheduler.ConfigureScheduler(Configuration);

#if DEBUG
            Logger.Information($"Starting DEBUG WebHost listening on {BIQuerySchedulerURL}:{BIQuerySchedulerDebugPort}.");
            CreateWebHostBuilderDebug(args).Build().Run();
#else
           
            Logger.Information($"Starting RELEASE WebHost listening on {BIQuerySchedulerURL}.");
            CreateWebHostBuilder(args).Build().RunAsService();            
#endif
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .UseKestrel()
                .UseContentRoot(pathToContentRoot)
                .UseUrls($"{BIQuerySchedulerURL}")
                .UseStartup<Startup>()
                .ConfigureServices( services => services.AddAutofac() )
                .UseSerilog();
        }

        public static IWebHostBuilder CreateWebHostBuilderDebug(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .UseUrls($"{BIQuerySchedulerURL}:{BIQuerySchedulerDebugPort}")
                .UseStartup<Startup>()
                .ConfigureServices(services => services.AddAutofac())
                .UseSerilog();
        }

        private static void ConfigureLogging()
        {
            string appName = Configuration["Name"] as string;
            if (string.IsNullOrEmpty(appName))
            {
                //throw new ConfigurationErrorsException("Name is not set in the web.config.");
            }

            string deploymentMode = Configuration["DeploymentMode"] as string;
            if (string.IsNullOrEmpty(deploymentMode))
            {
               // throw new ConfigurationErrorsException("DeploymentMode is not set in the web.config.");
            }

            string baseLogPath = Configuration["Logging:Path"] as string;
            if (string.IsNullOrEmpty(baseLogPath))
            {
               // throw new ConfigurationErrorsException("Logging:Path is not set in the web.config.");
            }

            var datadogAPIKey = Configuration["DatadogAPIKey"];
            if (string.IsNullOrWhiteSpace(datadogAPIKey))
            {
                throw new ConfigurationErrorsException("DatadogAPIKey is not set in the web.config.");
            }

            if (!CloudStorageAccount.TryParse(Configuration.GetConnectionString("AzureSerilogStorage"), out CloudStorageAccount azureStorageAccount))
            {
               // throw new ConfigurationErrorsException("AzureSerilogStorage Connection String is not set in the web.config.");
            }

            string outputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] <" + deploymentMode + "|{SourceContext}|{CorrelationId}> {Message}{NewLine}{NewLine}{Exception}{NewLine}";

            switch (deploymentMode.ToUpper())
            {
                case "LOCAL":
                    // This will only write to the local file system as this is the dev's local machine.
                    Log.Logger = new LoggerConfiguration()
                                 .Enrich.FromLogContext()
                                 .Enrich.WithMachineName()
                                 .Enrich.WithEnvironmentUserName()
                                 .MinimumLevel.Debug()
                                 .MinimumLevel.Override("System", LogEventLevel.Information)
                                 .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                                 .WriteTo.DatadogLogs(datadogAPIKey, "Serilog", appName, appName, new[] { $"environment:{deploymentMode}" }, logLevel: LogEventLevel.Warning)
                                 .CreateLogger();
                    break;

                case "DEV":
                case "QA":
                case "STAGE":
                case "TEST":
                case "DEMO":
                case "TRAINING":
                case "PROD":
                    Log.Logger = new LoggerConfiguration()
                                 .Enrich.FromLogContext()
                                 .Enrich.WithMachineName()
                                 .Enrich.WithEnvironmentUserName()
                                 .MinimumLevel.Information()
                                 .MinimumLevel.Override("System", LogEventLevel.Information)
                                 .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                                 .WriteTo.DatadogLogs(datadogAPIKey, "Serilog", appName, appName, new[] { $"environment:{deploymentMode}" }, logLevel: LogEventLevel.Warning)
                                 .CreateLogger();
                    break;

                default:
                    throw new IndexOutOfRangeException($"Unknown Deployment Mode found: {deploymentMode}");
            }

            if (deploymentMode.ToUpper() == "LOCAL")
            {
                Logger.Information($"Detailed log file will be written to files in {baseLogPath}");
                Logger.Information($"Detailed log data is also being sent to the Azure Table Storage Emulator in the table named {appName} using the account {azureStorageAccount.Credentials.AccountName}");
            }
            else
            {
                Logger.Information($"Detailed log data is being sent to the Azure Table Storage in the table named {appName} using the account {azureStorageAccount.Credentials.AccountName}");
            }

            Logger.Debug("Startup -> Logging Configuration: COMPLETE");
        }

        public static string GetLocalIPAddress()
        {
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }
    }
}
