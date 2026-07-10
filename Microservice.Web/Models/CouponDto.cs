namespace Microservice.Web.Models;

public record CouponDto(
    int CouponId,
    string CouponCode,
    double DiscountAmount,
    int MinAmount
);
