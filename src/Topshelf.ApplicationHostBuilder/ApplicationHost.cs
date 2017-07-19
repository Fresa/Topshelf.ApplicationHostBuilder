using System;
using System.Diagnostics;
using Microsoft.Win32;
using Topshelf.Hosts;
using Topshelf.Logging;
using Topshelf.Runtime;

namespace Topshelf.ApplicationHostBuilder
{
    public class ApplicationHost : Host, HostControl
    {
        private readonly LogWriter _log = HostLogger.Get<ConsoleRunHost>();
        private readonly ServiceHandle _serviceHandle;
        private readonly HostSettings _settings;
        private TopshelfExitCode _exitCode;

        public ApplicationHost(HostSettings settings, ServiceHandle serviceHandle)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));
            _settings = settings;
            _serviceHandle = serviceHandle;
            if (settings.CanSessionChanged)
            {
                SystemEvents.SessionSwitch += OnSessionChanged;
            }
            if (settings.CanHandlePowerEvent)
            {
                SystemEvents.PowerModeChanged += OnPowerModeChanged;
            }
        }

        private void OnSessionChanged(object sender, SessionSwitchEventArgs e)
        {
            _serviceHandle.SessionChanged(this, new ApplicationSessionChangedArguments(e.Reason));
        }

        private void OnPowerModeChanged(object sender, PowerModeChangedEventArgs e)
        {
            _serviceHandle.PowerEvent(this, new ApplicationPowerEventArguments(e.Mode));
        }

        public TopshelfExitCode Run()
        {
            try
            {
                _log.Debug("Starting application.");
                _exitCode = TopshelfExitCode.Ok;
                if (_serviceHandle.Start(this) == false)
                {
                    throw new TopshelfException("The application failed to start (return false).");
                }
                _log.InfoFormat("The {0} application is now running.", _settings.ServiceName);
            }
            catch (Exception ex)
            {
                var exceptionCallback = _settings.ExceptionCallback;
                if (exceptionCallback != null)
                {
                    Exception exception = ex;
                    exceptionCallback(exception);
                }
                _log.Error("An exception occurred", ex);
                HostLogger.Shutdown();
                return TopshelfExitCode.AbnormalExit;
            }
            return _exitCode;
        }

        void HostControl.RequestAdditionalTime(TimeSpan timeRemaining)
        {
        }

        void HostControl.Stop()
        {
            _log.Info("Application Stop requested, exiting.");
            StopService();
            HostLogger.Shutdown();
        }

        void HostControl.Restart()
        {
            _log.Info("Application Restart requested, but we don't support that here, so we are exiting.");
            ((HostControl)this).Stop();
        }

        private void StopService()
        {
            try
            {
                _log.InfoFormat("Stopping the {0} application", _settings.ServiceName);
                if (!_serviceHandle.Stop(this))
                    throw new TopshelfException("The application failed to stop (returned false).");
            }
            catch (Exception ex)
            {
                Action<Exception> exceptionCallback = _settings.ExceptionCallback;
                if (exceptionCallback != null)
                {
                    Exception exception = ex;
                    exceptionCallback(exception);
                }
                _log.Error("The application did not shut down gracefully", ex);
            }
            finally
            {
                _serviceHandle.Dispose();
                _log.InfoFormat("The {0} application has stopped.", _settings.ServiceName);
            }
        }

        private class ApplicationSessionChangedArguments : SessionChangedArguments
        {
            public SessionChangeReasonCode ReasonCode { get; }

            public int SessionId { get; }

            public ApplicationSessionChangedArguments(SessionSwitchReason reason)
            {
                ReasonCode = (SessionChangeReasonCode)Enum.ToObject(typeof(SessionChangeReasonCode), (int)reason);
                SessionId = Process.GetCurrentProcess().SessionId;
            }
        }

        private class ApplicationPowerEventArguments : PowerEventArguments
        {
            public PowerEventCode EventCode { get; }

            public ApplicationPowerEventArguments(PowerModes powerMode)
            {
                switch (powerMode)
                {
                    case PowerModes.Resume:
                        EventCode = PowerEventCode.ResumeAutomatic;
                        break;
                    case PowerModes.StatusChange:
                        EventCode = PowerEventCode.PowerStatusChange;
                        break;
                    case PowerModes.Suspend:
                        EventCode = PowerEventCode.Suspend;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(powerMode), powerMode, null);
                }
            }
        }
    }
}