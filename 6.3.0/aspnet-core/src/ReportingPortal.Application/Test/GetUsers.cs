﻿using Abp.Authorization;
using ReportingPortal.Models;
using ServicePortal.Core.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportingPortal.Test
{
    public class GetUsers : ReportingPortalAppServiceBase, IGetUsers
    {
        [AbpAuthorize(ReportingPermissions.ReportingPages_DemoUiComponents)]
        public List<ListUser> GetListUsers()
        {
            var listUser = new List<ListUser>()
            {
                new ListUser{Age=28,Name="Ali",Surname="Alizada"}
            };

            return listUser;
        }
    }
}
