using Microservices.Service.RewardAPI.Message;

namespace Microservices.Service.RewardAPI.Services;

public interface IRewardService
{
    public Task UpdateRewards(RewardMessage message);
}
