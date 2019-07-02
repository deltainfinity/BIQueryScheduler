using Autofac;
using Microsoft.Extensions.Configuration;
using Serilog;
using System.Reflection;
using Module = Autofac.Module;

namespace BIQueryScheduler.API
{
    /// <summary>
    /// Autofac Module for registering database connect, services, and repositories for DI
    /// </summary>
    public class BIQuerySchedulerCoreModule : Module
    {
        private static readonly ILogger Logger = Log.ForContext<BIQuerySchedulerCoreModule>();

        /// <summary>
        /// Default Constructor
        /// </summary>
        public BIQuerySchedulerCoreModule()
        { }

        /// <summary>
        /// DI Constructor
        /// </summary>
        /// <param name="configuration">The instance of Configuration setting to load</param>
        public BIQuerySchedulerCoreModule(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; set; }

        /// <summary>
        /// Override to add registrations to the container.
        /// </summary>
        /// <remarks>
        /// Note that the ContainerBuilder parameter is unique to this module.
        /// </remarks>
        /// <param name="builder">The builder through which components can be
        /// registered.</param>
        protected override void Load(ContainerBuilder builder)
        {
            // Register Services
            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
                .Where(t => t.Name.EndsWith("Service"))
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            Logger.Debug("Startup -> AutoFac BIQuerySchedulerCoreModule Module Registration: COMPLETE");
        }
    }
}
