using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Moq;
using Newtonsoft.Json;
using System.Security.Claims;


namespace wallet.tests;
public static class HttpContextMock
{
    public static Mock<IHttpContextAccessor> CreateMockHttpContextAccessor(Guid userId)
    {
        Mock<IHttpContextAccessor> mock = new Mock<IHttpContextAccessor>
        {
            CallBase = true
        };
        Mock<IServiceProvider> mock2 = new Mock<IServiceProvider>();
        IConfigurationRoot value = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>()).Build();
        mock2.Setup((IServiceProvider c) => c.GetService(typeof(IConfiguration))).Returns(value);
        mock.Setup((IHttpContextAccessor x) => x.HttpContext.RequestServices).Returns(mock2.Object);
        Claim item1 = new Claim(ClaimTypes.NameIdentifier, userId.ToString());

        List<Claim> list = new()
        {
            item1
        };

        mock.Setup((IHttpContextAccessor ca) => ca.HttpContext.User.Claims).Returns(list);
        return mock;
    }
}
