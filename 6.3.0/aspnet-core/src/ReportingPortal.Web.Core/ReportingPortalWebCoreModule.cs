using System;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Abp.AspNetCore;
using Abp.AspNetCore.Configuration;
using Abp.AspNetCore.SignalR;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.Zero.Configuration;
using ReportingPortal.Configuration;
using ReportingPortal.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.ApplicationParts;

namespace ReportingPortal
{
    [DependsOn(
         typeof(ReportingPortalApplicationModule),
         typeof(ReportingPortalEntityFrameworkModule),
         typeof(AbpAspNetCoreModule)
        , typeof(AbpAspNetCoreSignalRModule)
     )]
    public class ReportingPortalWebCoreModule : AbpModule
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfigurationRoot _appConfiguration;

        public ReportingPortalWebCoreModule(IWebHostEnvironment env)
        {
            _env = env;
            _appConfiguration = env.GetAppConfiguration();
        }

        public override void PreInitialize()
        {
            Configuration.DefaultNameOrConnectionString = _appConfiguration.GetConnectionString(
                ReportingPortalConsts.ConnectionStringName
            );

            // Use database for language management
            //Configuration.Modules.Zero().LanguageManagement.EnableDbLocalization();

            Configuration.Modules.AbpAspNetCore()
                 .CreateControllersForAppServices(
                     typeof(ReportingPortalApplicationModule).GetAssembly()
                 );

            //ConfigureTokenAuth();
        }

        //private void ConfigureTokenAuth()
        //{
        //    IocManager.Register<TokenAuthConfiguration>();
        //    var tokenAuthConfig = IocManager.Resolve<TokenAuthConfiguration>();

        //    tokenAuthConfig.SecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_appConfiguration["Authentication:JwtBearer:SecurityKey"]));
        //    tokenAuthConfig.Issuer = _appConfiguration["Authentication:JwtBearer:Issuer"];
        //    tokenAuthConfig.Audience = _appConfiguration["Authentication:JwtBearer:Audience"];
        //    tokenAuthConfig.SigningCredentials = new SigningCredentials(tokenAuthConfig.SecurityKey, SecurityAlgorithms.HmacSha256);
        //    tokenAuthConfig.Expiration = TimeSpan.FromDays(1);
        //}

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(ReportingPortalWebCoreModule).GetAssembly());
        }

        public override void PostInitialize()
        {
            IocManager.Resolve<ApplicationPartManager>()
                .AddApplicationPartsIfNotAddedBefore(typeof(ReportingPortalWebCoreModule).Assembly);
        }
    }
}
