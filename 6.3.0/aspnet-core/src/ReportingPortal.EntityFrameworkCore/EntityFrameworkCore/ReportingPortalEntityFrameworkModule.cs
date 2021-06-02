using Abp.EntityFrameworkCore;
using Abp.EntityFrameworkCore.Configuration;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.Zero.EntityFrameworkCore;

namespace ReportingPortal.EntityFrameworkCore
{
    [DependsOn(
        typeof(ReportingPortalCoreModule), 
        typeof(AbpEntityFrameworkCoreModule))]
    public class ReportingPortalEntityFrameworkModule : AbpModule
    {
        /* Used it tests to skip dbcontext registration, in order to use in-memory database of EF Core */
       // public bool SkipDbContextRegistration { get; set; }

       // public bool SkipDbSeed { get; set; }

        //public override void PreInitialize()
        //{
        //    if (!SkipDbContextRegistration)
        //    {
        //        Configuration.Modules.AbpEfCore().AddDbContext<ReportingPortalDbContext>(options =>
        //        {
        //            if (options.ExistingConnection != null)
        //            {
        //                ReportingPortalDbContextConfigurer.Configure(options.DbContextOptions, options.ExistingConnection);
        //            }
        //            else
        //            {
        //                ReportingPortalDbContextConfigurer.Configure(options.DbContextOptions, options.ConnectionString);
        //            }
        //        });
        //    }
        //}

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(ReportingPortalEntityFrameworkModule).GetAssembly());
        }

        //public override void PostInitialize()
        //{
        //    if (!SkipDbSeed)
        //    {
        //        SeedHelper.SeedHostDb(IocManager);
        //    }
        //}
    }
}
