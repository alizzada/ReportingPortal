using Abp.Application.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportingPortal.Caches
{
    public interface ICachingAppService:IApplicationService
    {
        Task ClearUserPermissionsCaches();
    }
}
