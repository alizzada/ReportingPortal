using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;

namespace ReportingPortal
{
    [DependsOn(
        typeof(ReportingPortalCoreModule), 
        typeof(AbpAutoMapperModule))]
    public class ReportingPortalApplicationModule : AbpModule
    {
        //public override void PreInitialize()
        //{
        //    Configuration.Authorization.Providers.Add<ReportingPortalAuthorizationProvider>();
        //}

        public override void Initialize()
        {
            var thisAssembly = typeof(ReportingPortalApplicationModule).GetAssembly();

            IocManager.RegisterAssemblyByConvention(thisAssembly);

            Configuration.Modules.AbpAutoMapper().Configurators.Add(
                // Scan the assembly for classes which inherit from AutoMapper.Profile
                cfg => cfg.AddMaps(thisAssembly)
            );
        }
    }
}
