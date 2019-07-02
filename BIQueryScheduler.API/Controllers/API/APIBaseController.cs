using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace BIQueryScheduler.Controllers.API
{
    public class APIBaseController : ControllerBase
    {
        protected IHostingEnvironment _hostingEnvironment;

        public APIBaseController(IHostingEnvironment environment)
        {
            _hostingEnvironment = environment;
        }
        
    }
}
