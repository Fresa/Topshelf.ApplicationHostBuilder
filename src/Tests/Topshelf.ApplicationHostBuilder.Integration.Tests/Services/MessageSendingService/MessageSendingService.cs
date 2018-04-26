using System;
using Test.It.Specifications;
using Test.It.While.Hosting.Your.Windows.Service;
using Topshelf.ApplicationHostBuilder.Integration.Tests.TestFramework;

namespace Topshelf.ApplicationHostBuilder.Integration.Tests.Services.MessageSendingService
{
    public class MessageSendingService : IWindowsService, IConfigurable
    {
        private readonly SimpleInjectorDependencyResolver _container;

        public MessageSendingService()
        {
            _container = new SimpleInjectorDependencyResolver();
        }

        public int Start(params string[] args)
        {
            var communicator = _container.Resolve<Communicator>();
            communicator.Send("Starting!");
            return 0;
        }

        public int Stop()
        {
            var communicator = _container.Resolve<Communicator>();
            communicator.Send("Stopping!");

            _container.Dispose();
            return 0;
        }

        public event Action<Exception> OnUnhandledException;

        public void Configure(ITestConfigurer reconfigurer)
        {
            _container.RegisterSingleton(() => new Communicator());

            _container.AllowOverridingRegistrations();
            reconfigurer.Configure(_container);
            _container.DisallowOverridingRegistrations();

            _container.Verify();
        }
    }
}