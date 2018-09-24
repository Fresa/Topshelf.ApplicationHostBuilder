using System;
using Topshelf.HostConfigurators;

namespace Topshelf.ApplicationHostBuilder
{
    public static class HostConfiguratorExtensions
    {
        public static void UseApplicationHostBuilder(this HostConfigurator hostConfigurator)
        {
            hostConfigurator.UseHostBuilder((environment, settings) => new ApplicationHostBuilder(environment, settings));
        }

        public static IDisposable UseApplicationHostBuilder(this HostConfigurator hostConfigurator, string[] commandLineArguments)
        {
            hostConfigurator.UseHostBuilder((environment, settings) => new ApplicationHostBuilder(environment, settings));
            var commandLine = string.Join(" ", commandLineArguments);
            return new DisposableAction(() => hostConfigurator.ApplyCommandLine(commandLine));            
        }

        private class DisposableAction : IDisposable
        {
            private readonly Action _action;

            public DisposableAction(Action action)
            {
                _action = action;
            }

            public void Dispose()
            {
                _action();
            }
        }
    }
}