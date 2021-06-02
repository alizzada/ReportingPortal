using System.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace ReportingPortal.EntityFrameworkCore
{
    public static class ReportingPortalDbContextConfigurer
    {
        public static void Configure(DbContextOptionsBuilder<ReportingPortalDbContext> builder, string connectionString)
        {
            builder.UseSqlServer(connectionString);
        }

        //public static void Configure(DbContextOptionsBuilder<ReportingPortalDbContext> builder, DbConnection connection)
        //{
        //    builder.UseSqlServer(connection);
        //}
    }
}
