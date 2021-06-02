using System;
using Castle.MicroKernel.Registration;
using NSubstitute;
using Abp.AutoMapper;
using Abp.Dependency;
using Abp.Modules;
using Abp.Configuration.Startup;
using Abp.Net.Mail;
using Abp.TestBase;
using Abp.Zero.Configuration;
using Abp.Zero.EntityFrameworkCore;
using ReportingPortal.EntityFrameworkCore;
using ReportingPortal.Tests.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Castle.Windsor.MsDependencyInjection;
using Microsoft.EntityFrameworkCore;
using Abp.Reflection.Extensions;

namespace ReportingPortal.Tests
{
    [DependsOn(
        typeof(ReportingPortalApplicationModule),
        typeof(ReportingPortalEntityFrameworkModule),
        typeof(AbpTestBaseModule)
        )]
    public class ReportingPortalTestModule : AbpModule
    {
        //public ReportingPortalTestModule(ReportingPortalEntityFrameworkModule abpProjectNameEntityFrameworkModule)
        //{
        //    abpProjectNameEntityFrameworkModule.SkipDbContextRegistration = true;
        //    abpProjectNameEntityFrameworkModule.SkipDbSeed = true;
        //}

        //public override void PreInitialize()
        //{
        //    Configuration.UnitOfWork.Timeout = TimeSpan.FromMinutes(30);
        //    Configuration.UnitOfWork.IsTransactional = false;

        //    // Disable static mapper usage since it breaks unit tests (see https://github.com/aspnetboilerplate/aspnetboilerplate/issues/2052)
        //    Configuration.Modules.AbpAutoMapper().UseStaticMapper = false;

        //    Configuration.BackgroundJobs.IsJobExecutionEnabled = false;

        //    // Use database for language management
        //    Configuration.Modules.Zero().LanguageManagement.EnableDbLocalization();

        //    RegisterFakeService<AbpZeroDbMigrator<ReportingPortalDbContext>>();

        //    Configuration.ReplaceService<IEmailSender, NullEmailSender>(DependencyLifeStyle.Transient);
        //}

        //public override void Initialize()
        //{
        //    ServiceCollectionRegistrar.Register(IocManager);
        //}

        //private void RegisterFakeService<TService>() where TService : class
        //{
        //    IocManager.IocContainer.Register(
        //        Component.For<TService>()
        //            .UsingFactoryMethod(() => Substitute.For<TService>())
        //            .LifestyleSingleton()
        //    );
        //}


        public override void PreInitialize()
        {
            Configuration.UnitOfWork.IsTransactional = false; //EF Core InMemory DB does not support transactions.
            SetupInMemoryDb();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(ReportingPortalTestModule).GetAssembly());
        }

        private void SetupInMemoryDb()
        {
            var services = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase();

            var serviceProvider = WindsorRegistrationHelper.CreateServiceProvider(
                IocManager.IocContainer,
                services
            );

            var builder = new DbContextOptionsBuilder<ReportingPortalDbContext>();
            builder.UseInMemoryDatabase("Test").UseInternalServiceProvider(serviceProvider);

            IocManager.IocContainer.Register(
                Component
                    .For<DbContextOptions<ReportingPortalDbContext>>()
                    .Instance(builder.Options)
                    .LifestyleSingleton()
            );
        }
    }
}
