using Microsoft.EntityFrameworkCore;
using QRCouponSystem.DbModels;
using QRCouponSystem.DTOs;
using QRCouponSystem.Services.Interfaces;

namespace QRCouponSystem.Services.Implementations
{
    public class WalletService : IWalletService
    {
        private readonly QrcouponSystemContext _context;

        public WalletService(QrcouponSystemContext context)
        {
            _context = context;
        }

        public async Task<WalletBalanceResponseDto> GetBalanceAsync(int userId)
        {
            var balance = await _context.Wallets
                .Where(x => x.UserId == userId)
                .Select(x => x.Balance)
                .FirstOrDefaultAsync();

            return new WalletBalanceResponseDto
            {
                AvailableBalance = (decimal)balance
            };
        }
    }
}
