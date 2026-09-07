namespace Microservices.Service.OrderAPI.Models.Dto;

public class StripeRequestDto
{
    public string SessionURL { get; set; }
    public string SessionId { get; set; }
    public string ApprovedURL { get; set; }
    public string CancelURL { get; set; }
    public OrderHeaderDto OrderHeader { get; set; }
}
