using System.Threading.Tasks;
using ReportingPortal.Configuration.Dto;

namespace ReportingPortal.Configuration
{
    public interface IConfigurationAppService
    {
        Task ChangeUiTheme(ChangeUiThemeInput input);
    }
}
