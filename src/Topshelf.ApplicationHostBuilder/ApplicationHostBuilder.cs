using System;
using Topshelf.Builders;
using Topshelf.Logging;
using Topshelf.Runtime;

namespace Topshelf.ApplicationHostBuilder
{
    public class ApplicationHostBuilder : HostBuilder
    {
        private readonly LogWriter _log = HostLogger.Get<ApplicationHostBuilder>();

        public HostEnvironment Environment { get; }

        public HostSettings Settings { get; }

        public ApplicationHostBuilder(HostEnvironment environment, HostSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));
            if (environment == null)
                throw new ArgumentNullException(nameof(environment));
            Environment = environment;
            Settings = settings;
        }

        public virtual Host Build(ServiceBuilder serviceBuilder)
        {
            return CreateHost(serviceBuilder.Build(Settings));
        }

        public void Match<T>(Action<T> callback) where T : class, HostBuilder
        {
            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback));
            }
            var obj = this as T;
            if (obj == null)
            {
                return;
            }
            callback(obj);
        }

        private Host CreateHost(ServiceHandle serviceHandle)
        {
            if (Environment.IsRunningAsAService)
            {
                _log.Debug("Running as a service, creating service host.");
                return Environment.CreateServiceHost(Settings, serviceHandle);
            }
            _log.Debug("Running as an application, creating the application host.");
            return new ApplicationHost(Settings, serviceHandle);
        }
    }
}