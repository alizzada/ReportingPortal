using Microsoft.EntityFrameworkCore;
using Abp.Zero.EntityFrameworkCore;
using Abp.EntityFrameworkCore;
using Abp.Authorization.Users;
using Abp.Organizations;

namespace ReportingPortal.EntityFrameworkCore
{
    public class ReportingPortalDbContext : AbpDbContext     //AbpZeroDbContext<Tenant, Role, User, ReportingPortalDbContext>
    {
        /* Define a DbSet for each entity of the application */
        public virtual DbSet<UserOrganizationUnit> UserOrganizationUnits { get; set; }
        //
        // Summary:
        //     OrganizationUnits.
        public virtual DbSet<OrganizationUnit> OrganizationUnits { get; set; }

        public ReportingPortalDbContext(DbContextOptions<ReportingPortalDbContext> options)
            : base(options)
        {
        }
    }
}
