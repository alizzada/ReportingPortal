using Abp.Authorization;
using Abp.Domain.Uow;
using Abp.Runtime.Caching;
using Abp.Zero.Configuration;
using Microsoft.Extensions.Configuration;
using ReportingPortal.Configuration;

namespace ReportingPortal.Authorization
{
    public class PermissionChecker : CustomPermissionChecker
    {
        public PermissionChecker(ICacheManager cacheManager, IUnitOfWorkManager unitOfWorkManager,IConfiguration configuration, IPermissionManager permissionManager,ICustomRoleManagementConfig roleManagementConfig)
            : base(cacheManager, unitOfWorkManager, configuration, roleManagementConfig)
        {
        }
    }
}
