using System;
using SimpleInjector;
using Test.It;

namespace Topshelf.ApplicationHostBuilder.Integration.Tests.TestFramework
{
    public class SimpleInjectorDependencyResolver : IServiceContainer
    {
        private readonly Container _container = new Container();

        public void Register<TImplementation>(Func<TImplementation> configurer)
            where TImplementation : class
        {
            _container.Register(configurer);
        }

        public void Register<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService
        {
            _container.Register<TService, TImplementation>();
        }

        public void RegisterSingleton<TImplementation>(Func<TImplementation> configurer)
            where TImplementation : class
        {
            _container.RegisterSingleton(configurer);
        }

        public void RegisterSingleton<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService
        {
            _container.RegisterSingleton<TService, TImplementation>();
        }

        public TService Resolve<TService>()
            where TService : class
        {
            return _container.GetInstance<TService>();
        }

        public void Verify()
        {
            _container.Verify();
        }

        public void Dispose()
        {
            _container.Dispose();
        }

        public void AllowOverridingRegistrations()
        {
            _container.Options.AllowOverridingRegistrations = true;
        }

        public void DisallowOverridingRegistrations()
        {
            _container.Options.AllowOverridingRegistrations = false;
        }
    }
}