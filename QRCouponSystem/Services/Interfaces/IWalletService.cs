using QRCouponSystem.DTOs;

namespace QRCouponSystem.Services.Interfaces
{
    public interface IWalletService
    {
        Task<WalletBalanceResponseDto> GetBalanceAsync(int userId);
    }
}
