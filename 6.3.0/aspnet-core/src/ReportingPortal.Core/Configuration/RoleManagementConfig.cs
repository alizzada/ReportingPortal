using Abp.MultiTenancy;
using Abp.Zero.Configuration;
using ServicePortal.Authorization.Roles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportingPortal.Configuration
{
    public class RoleManagementConfig : ICustomRoleManagementConfig
    {
        public List<StaticRoleDefinition> StaticRoles { get; }

        public RoleManagementConfig()
        {
            StaticRoles = new List<StaticRoleDefinition>
            {
                new StaticRoleDefinition(
                   StaticRoleNames.Host.Admin,
                   MultiTenancySides.Host),

                //Static tenant roles

                new StaticRoleDefinition(
                    StaticRoleNames.Tenants.Admin,
                    MultiTenancySides.Tenant),

                new StaticRoleDefinition(
                    StaticRoleNames.Tenants.User,
                    MultiTenancySides.Tenant)
            };
        }
    }
}
