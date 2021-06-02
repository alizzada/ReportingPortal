using Abp.Authorization;
using Abp.Localization;
using Abp.MultiTenancy;
using ServicePortal.Core.Shared;

namespace ReportingPortal.Authorization
{
    public class ReportingPortalAuthorizationProvider : AuthorizationProvider
    {
        public override void SetPermissions(IPermissionDefinitionContext context)
        {
            //COMMON PERMISSIONS (FOR BOTH OF TENANTS AND HOST)
            var reportingPages = context.GetPermissionOrNull(ReportingPermissions.ReportingPages) ?? context.CreatePermission(ReportingPermissions.ReportingPages, L("ReportingPages"));

            //TENANT-SPECIFIC PERMISSIONS
            reportingPages.CreateChildPermission(ReportingPermissions.ReportingPages_DemoUiComponents, L("DemoUiComponents"), multiTenancySides: MultiTenancySides.Tenant);
            reportingPages.CreateChildPermission(ReportingPermissions.ReportingPages_Administration_Languages, L("DemoUiComponents"), multiTenancySides: MultiTenancySides.Tenant);

            context.CreatePermission(PermissionNames.Pages_Users, L("Users"));
            context.CreatePermission(PermissionNames.Pages_Users_Activation, L("UsersActivation"));
            context.CreatePermission(PermissionNames.Pages_Roles, L("Roles"));
            context.CreatePermission(PermissionNames.Pages_Tenants, L("Tenants"), multiTenancySides: MultiTenancySides.Host);
        }

        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, ReportingPortalConsts.LocalizationSourceName);
        }
    }
}
