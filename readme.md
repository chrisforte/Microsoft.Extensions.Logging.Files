# Microsoft.Extensions.Logging.Files

File logger provider for Microsoft.Extensions.Logging that allows logging to local files in different formats.

## Usage  

In the `IHostBuilder` chained configuration methods, use `.AddFile<IFileLoggerBaseFormatter>(configuration, formatter)` under `IHostBuilder.ConfigureLogging()`.

```csharp
IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureLogging((hostBuilderContext, logging) =>
    {
        logging.ClearProviders();
        logging.AddFile<BasicFormatter>(
                        configuration =>
                        {
                            configuration.Directory = @"C:\Logs";
                            configuration.UseRollingFiles = true;
                            configuration.RollingFileTimestampFormat = @"yyyy-MM-dd";
                            configuration.FileExtension = @"log";
                            configuration.FileNamePrefix = @"MyLogFile";
                            configuration.MinimumLogLevel = LogLevel.Debug;
                        },
                        formatter =>
                        {
                            formatter.CaptureScopes = true;
                            formatter.UseUtcTimestamp = true;
                            formatter.IncludePID = true;
                            formatter.IncludeUser = true;
                        })
               .AddFile<CMTraceFormatter>(configuration => { })
               .AddFile<JSONFormatter>(configuration => { });
    })
    .Build();
```

Use the `configuration` delegate action to configure the base log file options, and the `formatter` delegate action to configure automatic formatting and filtering used by the `ILogger` instance.

Messages are processed and written immediately, and use a message processor and message queue to write to the underlying `FileStream`.

## Extensions

To create your own file logger format, create a class that implements the `FileLoggerBaseFormatter` class, and override the `Write()` method defined in `IFileLoggerBaseFormatter`. Pass that new class as a type parameter in `.AddFile<T>()`

```csharp
public abstract void Write<TState>(in LogEntry<TState> logEntry, IExternalScopeProvider scopeProvider, TextWriter textWriter);
```