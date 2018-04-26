using Test.It.Specifications;

namespace Topshelf.ApplicationHostBuilder.Integration.Tests.TestFramework
{
    public interface IConfigurable
    {
        void Configure(ITestConfigurer reconfigurer);
    }
}