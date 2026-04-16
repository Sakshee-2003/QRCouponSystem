using QRCouponSystem.DbModels;
using QRCouponSystem.DTOs;

namespace QRCouponSystem.Services.Interfaces
{
    public interface IAdminService
    {
        Task<CampaignResponseDto> CreateCampaignAsync(CampaignDto dto);
        Task<List<Coupon>> GenerateCoupons(int campaignId, GenerateCouponDto dto);
        Task<ReconcileResponseDto> ReconcileAsync();
    }
}
