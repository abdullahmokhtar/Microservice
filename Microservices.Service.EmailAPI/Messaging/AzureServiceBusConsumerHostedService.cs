namespace Microservices.Service.EmailAPI.Messaging;

public class AzureServiceBusConsumerHostedService : IHostedService
{
    private readonly IAzureServiceBusConsumer _serviceBusConsumer;
    private readonly ILogger<AzureServiceBusConsumerHostedService> _logger;

    public AzureServiceBusConsumerHostedService(IAzureServiceBusConsumer serviceBusConsumer, ILogger<AzureServiceBusConsumerHostedService> logger)
    {
        _serviceBusConsumer = serviceBusConsumer;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Azure Service Bus Consumer is starting.");
        await _serviceBusConsumer.Start();
        _logger.LogInformation("Azure Service Bus Consumer started successfully.");
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Azure Service Bus Consumer is stopping.");
        await _serviceBusConsumer.Stop();
        _logger.LogInformation("Azure Service Bus Consumer stopped successfully.");
    }
}
