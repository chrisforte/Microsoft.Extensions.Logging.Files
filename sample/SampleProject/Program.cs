using SampleProject;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureLogging((hostBuilderContext, logging) =>
    {
        logging.ClearProviders();
        logging.AddConsole()
               .AddDebug()
               .AddFile<BasicFormatter>(
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

await host.RunAsync();
