
* -> install **NLog.Extensions.Logging**

```cs - Startup.cs
using NLog;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        LogManager.LoadConfiguration(string.Concat(Directory.GetCurrentDirectory(), "/nlog.config"));
        // với config như "nlog.config" thì log sẽ được ghi vào file "~/internal_logs/internallog.txt"

        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddScoped<ILoggerManager, LoggerManager>();
        // ....
    }
}
```

```json - appsettings.json
{
    "Logging": {
        "LogLevel": {
            "Default": "Information",
            "Microsoft": "Warning",
            "Microsoft.Hosting.Lifetime": "Information"
        }
    }
}
```
```json - appsettings.Development.json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "System": "Information",
      "Microsoft": "Information"
    }
  }
}
```

```xml - nlog.config
<?xml version="1.0" encoding="utf-8" ?>
<nlog xmlns="http://www.nlog-project.org/schemas/NLog.xsd"
      xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
      autoReload="true"
      internalLogLevel="Trace"
      internalLogFile=".\internal_logs\internallog.txt">

	<targets>
		<target name="logfile" xsi:type="File"
				fileName=".\logs\${shortdate}_logfile.txt"
				layout="${longdate} ${level:uppercase=true} ${message}"/>
	</targets>

	<rules>
		<logger name="*" minlevel="Debug" writeTo="logfile" />
	</rules>
</nlog>

```

```cs - LoggerMangager.cs
using NLog;

public interface ILoggerManager
{
    void LogInfo(string message);
    void LogWarn(string message);
    void LogDebug(string message);
    void LogError(string message);
}

public class LoggerManager : ILoggerManager
{
    private static ILogger logger = LogManager.GetCurrentClassLogger();

    public LoggerManager()
    {
    }

    public void LogDebug(string message)
    {
        logger.Debug(message);
    }

    public void LogError(string message)
    {
        logger.Error(message);
    }

    public void LogInfo(string message)
    {
        logger.Info(message);
    }

    public void LogWarn(string message)
    {
        logger.Warn(message);
    }
}
```