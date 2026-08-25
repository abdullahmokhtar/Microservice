using System.Text;
using System.Text.Json;
using Azure.Messaging.ServiceBus;

namespace Microservice.MessageBus;

public class MessageBus : IMessageBus
{
    public async Task PublishMessage(string connectionString, object message, string topic_queue_Name)
    {
        await using var sender = new ServiceBusClient(connectionString).CreateSender(topic_queue_Name);

        var jsonMessage = JsonSerializer.Serialize(message);
        var finalMessage = new ServiceBusMessage(Encoding.UTF8.GetBytes(jsonMessage))
        {
            CorrelationId = Guid.NewGuid().ToString()
        };

        await sender.SendMessageAsync(finalMessage);
        await sender.DisposeAsync();
    }
}
