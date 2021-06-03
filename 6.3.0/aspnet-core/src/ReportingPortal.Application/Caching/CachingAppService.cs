using Abp.Authorization;
using Abp.Runtime.Caching;
using ServicePortal.Core.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportingPortal.Caches
{
    [AbpAuthorize(ReportingPermissions.ReportingPages_DemoUiComponents)]
    public class CachingAppService : ReportingPortalAppServiceBase, ICachingAppService
    {
        private readonly ICacheManager _cacheManager;

        public CachingAppService(ICacheManager cacheManager)
        {
            _cacheManager = cacheManager;
        }
        public async Task ClearUserPermissionsCaches()
        {
            var cache = _cacheManager.GetUserPermissionCache();
            
            await cache.ClearAsync();
        }
    }
}
