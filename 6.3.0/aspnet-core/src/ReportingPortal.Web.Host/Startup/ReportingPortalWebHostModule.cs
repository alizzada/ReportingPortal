using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Abp.Modules;
using Abp.Reflection.Extensions;
using ReportingPortal.Configuration;

namespace ReportingPortal.Web.Host.Startup
{
    [DependsOn(
       typeof(ReportingPortalWebCoreModule))]
    public class ReportingPortalWebHostModule : AbpModule
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfigurationRoot _appConfiguration;

        public ReportingPortalWebHostModule(IWebHostEnvironment env)
        {
            _env = env;
            _appConfiguration = env.GetAppConfiguration();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(ReportingPortalWebHostModule).GetAssembly());
        }
    }
}
