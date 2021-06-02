using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportingPortal.Configuration
{
    public interface ICustomRoleManagementConfig
    {
        List<StaticRoleDefinition> StaticRoles { get; }
    }
}
