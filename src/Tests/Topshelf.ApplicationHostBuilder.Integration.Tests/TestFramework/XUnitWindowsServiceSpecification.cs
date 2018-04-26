using Test.It.While.Hosting.Your.Windows.Service;

namespace Topshelf.ApplicationHostBuilder.Integration.Tests.TestFramework
{
    public abstract class XUnitWindowsServiceSpecification<TApplicationBuilder> : WindowsServiceSpecification<DefaultWindowsServiceHostStarter<TApplicationBuilder>> 
        where TApplicationBuilder : IWindowsServiceBuilder, new()
    {
        protected XUnitWindowsServiceSpecification()
        {
            SetConfiguration(new DefaultWindowsServiceHostStarter<TApplicationBuilder>());
        }
    }
}