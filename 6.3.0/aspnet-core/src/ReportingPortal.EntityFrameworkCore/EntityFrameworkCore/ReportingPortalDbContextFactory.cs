using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using ReportingPortal.Configuration;
using ReportingPortal.Web;

namespace ReportingPortal.EntityFrameworkCore
{
    /* This class is needed to run "dotnet ef ..." commands from command line on development. Not used anywhere else */
    public class ReportingPortalDbContextFactory : IDesignTimeDbContextFactory<ReportingPortalDbContext>
    {
        public ReportingPortalDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<ReportingPortalDbContext>();
            var configuration = AppConfigurations.Get(WebContentDirectoryFinder.CalculateContentRootFolder());

            ReportingPortalDbContextConfigurer.Configure(builder, configuration.GetConnectionString(ReportingPortalConsts.ConnectionStringName));

            return new ReportingPortalDbContext(builder.Options);
        }
    }
}
