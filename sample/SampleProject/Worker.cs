namespace SampleProject
{
    public class Worker : BackgroundService
    {
        private readonly IHostApplicationLifetime _applicationLifetime;
        private readonly ILogger<Worker> _logger;

        public Worker(IHostApplicationLifetime applicationLifetime, ILogger<Worker> logger)
        {
            _applicationLifetime = applicationLifetime;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(5000, stoppingToken);

                break;
            }

            _applicationLifetime.StopApplication();
        }
    }
}