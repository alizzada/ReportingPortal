using Abp.Application.Services;
using ReportingPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportingPortal.Test
{
    public interface IGetUsers : IApplicationService
    {
        List<ListUser> GetListUsers();
    }
}
