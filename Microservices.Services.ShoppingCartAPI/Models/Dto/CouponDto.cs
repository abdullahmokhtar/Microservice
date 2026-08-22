namespace Microservices.Services.ShoppingCartAPI.Models.Dto;

public record CouponDto(
    int CouponId,
    string CouponCode,
    decimal DiscountAmount,
    int MinAmount
);
