using System.Threading.Tasks;
using ReportingPortal.Models.TokenAuth;
using ReportingPortal.Web.Controllers;
using Shouldly;
using Xunit;

namespace ReportingPortal.Web.Tests.Controllers
{
    public class HomeController_Tests: ReportingPortalWebTestBase
    {
        [Fact]
        public async Task Index_Test()
        {
            await AuthenticateAsync(null, new AuthenticateModel
            {
                UserNameOrEmailAddress = "admin",
                Password = "123qwe"
            });

            //Act
            var response = await GetResponseAsStringAsync(
                GetUrl<HomeController>(nameof(HomeController.Index))
            );

            //Assert
            response.ShouldNotBeNullOrEmpty();
        }
    }
}