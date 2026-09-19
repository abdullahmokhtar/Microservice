namespace Microservices.Service.EmailAPI.Utlity;

public class TopicAndQueueNames
{
    public string EmailShoppingCart { get; set; } = null!;
    public string UserRegistration { get; set; } = null!;
    public string OrderCreatedTopic { get; set; } = null!;
    public string OrderCreatedEmailSubscription { get; set; } = null!;
}
