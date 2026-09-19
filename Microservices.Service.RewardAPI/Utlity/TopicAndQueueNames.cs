namespace Microservices.Service.RewardAPI.Utlity;

public class TopicAndQueueNames
{
    public string OrderCreatedTopic { get; set; } = null!;
    public string OrderCreatedRewardSubscription { get; set; } = null!;
}
