namespace Microservice.MessageBus;

public interface IMessageBus
{
    Task PublishMessage(string connectionString, object message, string topic_queue_Name);
}
