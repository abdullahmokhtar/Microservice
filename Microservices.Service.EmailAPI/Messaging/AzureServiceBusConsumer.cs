using System.Text;
using System.Text.Json;
using Azure.Messaging.ServiceBus;
using Microservices.Service.EmailAPI.Message;
using Microservices.Service.EmailAPI.Models;
using Microservices.Service.EmailAPI.Models.DTO;
using Microservices.Service.EmailAPI.Utlity;
using Microservices.Services.EmailAPI.Data;
using Microsoft.Extensions.Options;

namespace Microservices.Service.EmailAPI.Messaging;

public class AzureServiceBusConsumer : IAzureServiceBusConsumer
{
    private readonly ServiceBusProcessor _emailCartProcessor;
    private readonly ServiceBusProcessor _userRegistrationProcessor;
    private readonly ServiceBusProcessor _OrderPlacedProcessor;
    private readonly IServiceProvider _serviceProvider;

    public AzureServiceBusConsumer(IOptions<AzureConfig> azureConfig, IOptions<TopicAndQueueNames> topicAndQueueNames, IServiceProvider serviceProvider)
    {
        var client = new ServiceBusClient(azureConfig.Value.ConnectionString);
        _emailCartProcessor = client.CreateProcessor(topicAndQueueNames.Value.EmailShoppingCart);
        _userRegistrationProcessor = client.CreateProcessor(topicAndQueueNames.Value.UserRegistration);
        _OrderPlacedProcessor = client.CreateProcessor(topicAndQueueNames.Value.OrderCreatedTopic, topicAndQueueNames.Value.OrderCreatedEmailSubscription);
        _serviceProvider = serviceProvider;
    }

    public async Task Start()
    {
        _emailCartProcessor.ProcessMessageAsync += OnEmailCartRequestReceived;
        _emailCartProcessor.ProcessErrorAsync += ErrorHandler;
        await _emailCartProcessor.StartProcessingAsync();
        _userRegistrationProcessor.ProcessMessageAsync += OnUserRegistrationRequestReceived;
        _userRegistrationProcessor.ProcessErrorAsync += ErrorHandler;
        await _userRegistrationProcessor.StartProcessingAsync();
        _OrderPlacedProcessor.ProcessMessageAsync += OnOrderPlacedRequestReceived;
        _OrderPlacedProcessor.ProcessErrorAsync += ErrorHandler;
        await _OrderPlacedProcessor.StartProcessingAsync();
    }

    private async Task OnOrderPlacedRequestReceived(ProcessMessageEventArgs args)
    {
        var body = Encoding.UTF8.GetString(args.Message.Body);
        var rewardMessage = JsonSerializer.Deserialize<RewardMessage>(body);

        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var emailLog = new EmailLogger
        {
            Email = rewardMessage.UserId,
            Message = "Thank you for Ordering with us.",
            SentAt = DateTime.UtcNow
        };


        context.EmailLoggers.Add(emailLog);
        await context.SaveChangesAsync();
        await args.CompleteMessageAsync(args.Message);
    }

    private async Task OnUserRegistrationRequestReceived(ProcessMessageEventArgs args)
    {
        var body = Encoding.UTF8.GetString(args.Message.Body);
        var email = JsonSerializer.Deserialize<string>(body);

        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var emailLog = new EmailLogger
        {
            Email = email,
            Message = "Thank you for registering with us.",
            SentAt = DateTime.UtcNow
        };


        context.EmailLoggers.Add(emailLog);
        await context.SaveChangesAsync();
        await args.CompleteMessageAsync(args.Message);
    }

    private Task ErrorHandler(ProcessErrorEventArgs args)
    {
        Console.WriteLine(args.Exception.ToString());
        return Task.CompletedTask;
    }

    private async Task OnEmailCartRequestReceived(ProcessMessageEventArgs args)
    {
        var body = Encoding.UTF8.GetString(args.Message.Body);
        var cart = JsonSerializer.Deserialize<CartDto>(body);

        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var emailLog = new EmailLogger
        {
            Email = cart?.CartHeader.Email,
            Message = $"Your cart total is {cart?.CartHeader.CartTotal}. Thank you for shopping with us!",
            SentAt = DateTime.UtcNow
        };

        context.EmailLoggers.Add(emailLog);
        await context.SaveChangesAsync();
        await args.CompleteMessageAsync(args.Message);
    }

    public async Task Stop()
    {
        await _emailCartProcessor.StopProcessingAsync();
        await _emailCartProcessor.DisposeAsync();

        await _userRegistrationProcessor.StopProcessingAsync();
        await _userRegistrationProcessor.DisposeAsync();
    }
}
