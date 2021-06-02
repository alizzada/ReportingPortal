using Abp.AspNetCore;
using Abp.AspNetCore.TestBase;
using Abp.Modules;
using Abp.Reflection.Extensions;
using ReportingPortal.EntityFrameworkCore;
using ReportingPortal.Web.Startup;
using Microsoft.AspNetCore.Mvc.ApplicationParts;

namespace ReportingPortal.Web.Tests
{
    [DependsOn(
        typeof(ReportingPortalWebMvcModule),
        typeof(AbpAspNetCoreTestBaseModule)
    )]
    public class ReportingPortalWebTestModule : AbpModule
    {
        public ReportingPortalWebTestModule(ReportingPortalEntityFrameworkModule abpProjectNameEntityFrameworkModule)
        {
            abpProjectNameEntityFrameworkModule.SkipDbContextRegistration = true;
        } 
        
        public override void PreInitialize()
        {
            Configuration.UnitOfWork.IsTransactional = false; //EF Core InMemory DB does not support transactions.
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(ReportingPortalWebTestModule).GetAssembly());
        }
        
        public override void PostInitialize()
        {
            IocManager.Resolve<ApplicationPartManager>()
                .AddApplicationPartsIfNotAddedBefore(typeof(ReportingPortalWebMvcModule).Assembly);
        }
    }
}