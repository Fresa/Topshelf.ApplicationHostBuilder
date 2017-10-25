# Topshelf.ApplicationHostBuilder
Extension for <a href="http://topshelf-project.com" target="_blank">Topshelf</a> to be able to use any type of application, not just a Console Application.

## Why?
Topshelf requires that you create a project with output type Console Application. This extension removes that requirement. You can use any output type. You can even host your application during unit testing!

## Download
https://www.nuget.org/packages/Topshelf.ApplicationHostBuilder/

## Release Notes
**1.0.1** Incorrect logger for ApplicationHost

## Usage
Use the extension method `UseApplicationHostBuilder` for the host configurator you'll get by initializing the Topshelf `HostFactory`.

## Example
```
HostFactory.Run(config =>
{
    config.UseApplicationHostBuilder();

    config.Service<MyService>(configurator =>
    {
        configurator.WhenStarted((service, control) => service.Start());
        configurator.WhenStopped(service => service.Stop());
    });
});
```

## Caveats
There are some minor caveats.

### Test Runners
When using ReSharper's test runner, it will add some arguments when bootstrapping the test assembly. Topshelf parses input arguments by getting them from `Environment.GetCommandLineArgs()`. It will pick up the arguments from ReSharpers test runner and fail creating the hosting environment. Fortunately they provide a function to override the command line. Use the `ApplyCommandLine` function on the host configurator supplied by the `HostFactory` to simulate the behaviour you are testing.

### Console Usage
The `HelpHost` and the `TopshelfConsoleTraceListener` within Topshelf uses `Console`'s write functions directly, which will not make any sense if you do not have an application that listens to `Console`'s output. Fortunately `Console` exposes a function (`SetOut`) making it possible to redirect text being written to `Console` by rolling your own `TextWriter`.

### Stopping the Application
- When installing the application as a Windows Service, it works just as described in Topshelf's documentation.
- When running it through an automated test, you probably want to use the host controller provided by Topshelf after the hosting has started. See `WhenStarted` function for `ServiceConfigurator<T>`.
- When running it from a console/file explorer you need to kill the application from the Task Manager. For now, there is no way to do a graceful stop.
- When debugging from Visual Studio, just hit Stop Debugging. For now, there is no way to do a graceful stop.
