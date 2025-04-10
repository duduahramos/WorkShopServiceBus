using Azure.Messaging.ServiceBus;

namespace Workshop.ServiceBus.Worker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _configuration;
        private ServiceBusProcessor _processor;

        public Worker(ILogger<Worker> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            var connectionString = _configuration["ServiceBus:ConnectionString"];

            var queueName = _configuration["ServiceBus:QueueName"];

            var client = new ServiceBusClient(connectionString);

            _processor = client.CreateProcessor(queueName, new ServiceBusProcessorOptions());

            _processor.ProcessMessageAsync += MessageHandler;

            _processor.ProcessErrorAsync += ErrorHandler;

            await _processor.StartProcessingAsync(cancellationToken);
        }

        private async Task MessageHandler(ProcessMessageEventArgs args)
        {
            string body = args.Message.Body.ToString();

            _logger.LogInformation($"Mensagem processada: {body}");

            await args.CompleteMessageAsync(args.Message);
        }

        private Task ErrorHandler(ProcessErrorEventArgs args)
        {
            _logger.LogError(args.Exception, "Erro ao processar a mensagem");

            return Task.CompletedTask;
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await _processor.StopProcessingAsync(cancellationToken);
            await _processor.DisposeAsync();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.CompletedTask;
        }
    }
}