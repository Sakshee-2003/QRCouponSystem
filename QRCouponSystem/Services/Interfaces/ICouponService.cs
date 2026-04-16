using QRCouponSystem.DTOs;

namespace QRCouponSystem.Services.Interfaces
{
    public interface ICouponService
    {
        Task<List<CouponDto>> GetAvailableCouponsAsync();
        Task<CouponRedeemResponseDto> RedeemCouponAsync(int userId, CouponRedeemRequestDto request);
    }
}
