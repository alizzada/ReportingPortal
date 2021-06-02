using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Runtime.Session;
using ReportingPortal.Configuration.Dto;

namespace ReportingPortal.Configuration
{
    //[AbpAuthorize]
    public class ConfigurationAppService : ReportingPortalAppServiceBase, IConfigurationAppService
    {
        public async Task ChangeUiTheme(ChangeUiThemeInput input)
        {
            await SettingManager.ChangeSettingForUserAsync(AbpSession.ToUserIdentifier(), AppSettingNames.UiTheme, input.Theme);
        }
    }
}
