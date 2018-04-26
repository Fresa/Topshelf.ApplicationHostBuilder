using Test.It.Specifications;
using Test.It.While.Hosting.Your.Windows.Service;

namespace Topshelf.ApplicationHostBuilder.Integration.Tests.TestFramework
{
    public class TopshelfHostedWindowsServiceBuilder<TService> : DefaultWindowsServiceBuilder 
        where TService : class, IWindowsService, IConfigurable, new()
    {
        public override IWindowsService Create(ITestConfigurer configurer)
        {
            var service = new TService();
            service.Configure(configurer);
            return new TopshelfHostService<TService>(service);
        }
    }
}