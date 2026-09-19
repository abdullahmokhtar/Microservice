using System.Text;
using System.Text.Json;
using Azure.Messaging.ServiceBus;
using Microservices.Service.RewardAPI.Message;
using Microservices.Service.RewardAPI.Services;
using Microservices.Service.RewardAPI.Utlity;
using Microsoft.Extensions.Options;

namespace Microservices.Service.RewardAPI.Messaging;

public class AzureServiceBusConsumer : IAzureServiceBusConsumer
{
    private readonly ServiceBusProcessor _rewardProcessor;
    private readonly IServiceProvider _serviceProvider;

    public AzureServiceBusConsumer(IOptions<AzureConfig> azureConfig, IOptions<TopicAndQueueNames> topicAndQueueNames, IServiceProvider serviceProvider)
    {
        var client = new ServiceBusClient(azureConfig.Value.ConnectionString);
        _rewardProcessor = client.CreateProcessor(topicAndQueueNames.Value.OrderCreatedTopic, topicAndQueueNames.Value.OrderCreatedRewardSubscription);
        _serviceProvider = serviceProvider;
    }

    public async Task Start()
    {
        _rewardProcessor.ProcessMessageAsync += OnRewardRequestReceived;
        _rewardProcessor.ProcessErrorAsync += ErrorHandler;
        await _rewardProcessor.StartProcessingAsync();
    }

    private Task ErrorHandler(ProcessErrorEventArgs args)
    {
        Console.WriteLine(args.Exception.ToString());
        return Task.CompletedTask;
    }

    private async Task OnRewardRequestReceived(ProcessMessageEventArgs args)
    {
        var body = Encoding.UTF8.GetString(args.Message.Body);
        var rewardMessage = JsonSerializer.Deserialize<RewardMessage>(body);

        using var scope = _serviceProvider.CreateScope();
        var rewardService = scope.ServiceProvider.GetRequiredService<IRewardService>();

        await rewardService.UpdateRewards(rewardMessage);
        await args.CompleteMessageAsync(args.Message);
    }

    public async Task Stop()
    {
        await _rewardProcessor.StopProcessingAsync();
        await _rewardProcessor.DisposeAsync();
    }
}
