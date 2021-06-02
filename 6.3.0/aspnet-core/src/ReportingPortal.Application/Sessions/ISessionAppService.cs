using System.Threading.Tasks;
using Abp.Application.Services;
using ReportingPortal.Sessions.Dto;

namespace ReportingPortal.Sessions
{
    public interface ISessionAppService : IApplicationService
    {
        Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations();
    }
}
