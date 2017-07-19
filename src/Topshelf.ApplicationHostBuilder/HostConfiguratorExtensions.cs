using Topshelf.HostConfigurators;

namespace Topshelf.ApplicationHostBuilder
{
    public static class HostConfiguratorExtensions
    {
        public static void UseApplicationHostBuilder(this HostConfigurator hostConfigurator)
        {
            hostConfigurator.UseHostBuilder((environment, settings) => new ApplicationHostBuilder(environment, settings));
        }
    }
}