using System;
using Test.It.While.Hosting.Your.Windows.Service;

namespace Topshelf.ApplicationHostBuilder.Integration.Tests.TestFramework
{
    internal class TopshelfHostService<TService> : IWindowsService 
        where TService : class, IWindowsService
    {
        private HostControl _control;
        private readonly TService _service;

        public TopshelfHostService(TService service)
        {
            _service = service;
            _service.OnUnhandledException += exception => OnUnhandledException?.Invoke(exception);
        }

        public int Start(params string[] args)
        {
            var startCode = (int)HostFactory.Run(config =>
            {
                config.ApplyCommandLine("");
                config.UseApplicationHostBuilder();
                config.OnException(exception => OnUnhandledException?.Invoke(exception));

                config.Service<TService>(settings =>
                {
                    settings.ConstructUsing(hostSettings => _service);
                    settings.WhenStarted((service, control) =>
                    {
                        _control = control;
                        var code = service.Start();
                        return code == 0;
                    });
                    settings.WhenStopped(service =>
                    {
                        service.Stop();
                    });
                });
            });
            return startCode;
        }

        public int Stop()
        {
            if (_control == null)
            {
                return -1;
            }
            _control.Stop();
            return 0;
        }

        public event Action<Exception> OnUnhandledException;
    }
}