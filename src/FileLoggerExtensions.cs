public static class FileLoggerExtensions
{
    /// <summary>
    /// Add a default <see cref="FileLogger"/> instance to the logging configuration.
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static ILoggingBuilder AddFile(this ILoggingBuilder builder)
    {
        builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, FileLoggerProvider>());
        LoggerProviderOptions.RegisterProviderOptions<FileLoggerOptions, FileLoggerProvider>(builder.Services);

        return builder;
    }

    /// <summary>
    /// Add a <see cref="FileLogger"/> instance to the logging configuration with the supplied <see cref="FileLoggerOptions">options</see>.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="configuration">Options builder for this instance.</param>
    /// <returns></returns>
    public static ILoggingBuilder AddFile(this ILoggingBuilder builder, Action<FileLoggerOptions> configuration)
    {
        builder.AddFile();
        builder.Services.Configure<FileLoggerOptions>(configuration);

        return builder;
    }

    /// <summary>
    /// Add a <see cref="FileLogger"/> instance to the logging configuration with the supplied <see cref="FileLoggerOptions">options</see>, 
    /// using the specified <see cref="FileLoggerBaseFormatter">formatter</see> implementation.
    /// </summary>
    /// <typeparam name="TFormatter"></typeparam>
    /// <param name="builder"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static ILoggingBuilder AddFile<TFormatter>(this ILoggingBuilder builder, Action<FileLoggerOptions> configuration) where TFormatter : FileLoggerBaseFormatter
    {
        builder.AddFile();
        builder.Services.Configure<FileLoggerOptions>(configuration);

        builder.AddConfiguration();
        builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<FileLoggerBaseFormatter, TFormatter>());

        return builder;
    }

    /// <summary>
    /// Add a <see cref="FileLogger"/> instance to the logging configuration with the supplied <see cref="FileLoggerOptions">options</see>, 
    /// using the specified <see cref="FileLoggerBaseFormatter">formatter</see> implementation with supplied <see cref="FileLoggerFormatterOptions">formatter options</see>.
    /// </summary>
    /// <typeparam name="TFormatter"></typeparam>
    /// <param name="builder"></param>
    /// <param name="configuration"></param>
    /// <param name="formatter"></param>
    /// <returns></returns>
    public static ILoggingBuilder AddFile<TFormatter>(this ILoggingBuilder builder, Action<FileLoggerOptions> configuration, Action<FileLoggerFormatterOptions> formatter) where TFormatter : FileLoggerBaseFormatter
    {
        builder.AddFile();
        builder.Services.Configure<FileLoggerOptions>(configuration);

        builder.AddConfiguration();
        builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<FileLoggerBaseFormatter, TFormatter>());
        builder.Services.Configure<FileLoggerFormatterOptions>(formatter);

        return builder;
    }
}