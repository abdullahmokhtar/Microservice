using Microservices.Service.RewardAPI.Data;
using Microservices.Service.RewardAPI.Message;
using Microservices.Service.RewardAPI.Models;

namespace Microservices.Service.RewardAPI.Services;

public class RewardService(IServiceProvider serviceProvider) : IRewardService
{
    public async Task UpdateRewards(RewardMessage message)
    {
        var reward = new Reward
        {
            UserId = message.UserId,
            RewardActivity = message.RewardActivity,
            OrderId = message.OrderId,
            RewardDate = DateTime.UtcNow
        };

        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        context.Rewards.Add(reward);
        await context.SaveChangesAsync();
    }
}
