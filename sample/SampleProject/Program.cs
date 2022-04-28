using SampleProject;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureLogging((hostBuilderContext, logging) =>
    {
        logging.ClearProviders();
        logging.AddConsole()
               .AddDebug()
               .AddFile<CMTraceFormatter>(configure =>
               {
                   configure.Directory = Path.GetTempPath();
               });
    })
    .ConfigureServices((builder, services) =>
    {
        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();
