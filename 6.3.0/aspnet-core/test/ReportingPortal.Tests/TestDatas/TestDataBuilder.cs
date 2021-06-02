using ReportingPortal.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportingPortal.Tests.TestDatas
{
    public  class TestDataBuilder
    {
        private readonly ReportingPortalDbContext _context;

        public TestDataBuilder(ReportingPortalDbContext context)
        {
            _context = context;
        }

        public void Build()
        {
            //create test data here...
        }
    }
}
