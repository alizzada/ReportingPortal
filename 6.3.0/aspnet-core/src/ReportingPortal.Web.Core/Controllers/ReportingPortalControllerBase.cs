using Abp.AspNetCore.Mvc.Controllers;
using Abp.IdentityFramework;
using Microsoft.AspNetCore.Identity;

namespace ReportingPortal.Controllers
{
    public abstract class ReportingPortalControllerBase: AbpController
    {
        protected ReportingPortalControllerBase()
        {
            LocalizationSourceName = ReportingPortalConsts.LocalizationSourceName;
        }

        protected void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }
    }
}
