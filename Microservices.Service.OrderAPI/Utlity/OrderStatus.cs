namespace Microservices.Service.OrderAPI.Utlity;

public enum OrderStatus
{
    Pending,
    Approved,
    ReadyForPickup,
    Completed,
    Refunded,
    Cancelled,
}
