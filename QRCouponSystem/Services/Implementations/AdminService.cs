using Microsoft.EntityFrameworkCore;
using QRCouponSystem.DbModels;
using QRCouponSystem.DTOs;
using QRCouponSystem.Services.Interfaces;

namespace QRCouponSystem.Services.Implementations
{
    public class AdminService: IAdminService
    {
        private readonly QrcouponSystemContext _context;

        public AdminService(QrcouponSystemContext context)
        {
            _context = context;
        }
        public async Task<CampaignResponseDto> CreateCampaignAsync(CampaignDto dto)
        {
            var campaign = new Campaign
            {
                Name = dto.Name,
                ExpiryDate = dto.ExpiryDate
            };

            _context.Campaigns.Add(campaign);
            await _context.SaveChangesAsync();

            return new CampaignResponseDto
            {
                Id = campaign.Id,
                Name = campaign.Name,
                ExpiryDate = (DateTime)campaign.ExpiryDate
            };
        }

        public async Task<List<Coupon>> GenerateCoupons(int campaignId, GenerateCouponDto dto)
        {
            var campaign = await _context.Campaigns.FirstOrDefaultAsync(x => x.Id == campaignId);

            if (campaign == null)
                throw new Exception("Campaign not found");

            var coupons = new List<Coupon>();

            for (int i = 0; i < dto.Count; i++)
            {
                coupons.Add(new Coupon
                {
                    Code = Guid.NewGuid().ToString("N"),
                    CampaignId = campaignId,
                    Value = dto.Value,
                    IsRedeemed = false,
                    ExpiryDate = campaign.ExpiryDate
                });
            }

            _context.Coupons.AddRange(coupons);
            await _context.SaveChangesAsync();

            return coupons;
        }

        public async Task<ReconcileResponseDto> ReconcileAsync()
        {
            var transactions = await _context.Transactions
                .Where(t => t.Status == "Success")
                .ToListAsync();

            int couponsFixed = 0;
            int walletsFixed = 0;

            foreach (var tx in transactions)
            {
                var coupon = await _context.Coupons.FindAsync(tx.CouponId);
                var wallet = await _context.Wallets.FirstOrDefaultAsync(w => w.UserId == tx.UserId);

                if (coupon == null || wallet == null)
                    continue;
                
                if (!coupon.IsRedeemed)
                {
                    coupon.IsRedeemed = true;
                    couponsFixed++;
                }

                var expectedBalance = await _context.Transactions
                    .Where(t => t.UserId == tx.UserId && t.Status == "Success")
                    .SumAsync(t => t.Amount);

                if (wallet.Balance != expectedBalance)
                {
                    wallet.Balance = expectedBalance;
                    walletsFixed++;
                }
            }

            await _context.SaveChangesAsync();

            return new ReconcileResponseDto
            {
                TotalTransactionsChecked = transactions.Count,
                CouponsFixed = couponsFixed,
                WalletsCorrected = walletsFixed,
                Message = "Reconciliation completed successfully"
            };
        }
    }
}
