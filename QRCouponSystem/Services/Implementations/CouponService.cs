using Microsoft.EntityFrameworkCore;
using QRCouponSystem.DbModels;
using QRCouponSystem.DTOs;
using QRCouponSystem.Enums;
using QRCouponSystem.Services.Interfaces;
using System.Transactions;

namespace QRCouponSystem.Services.Implementations
{
    public class CouponService: ICouponService
    {
        private readonly QrcouponSystemContext _context;

        public CouponService(QrcouponSystemContext context)
        {
            _context = context;
        }
        public async Task<List<CouponDto>> GetAvailableCouponsAsync()
        {
            var coupons = await _context.Coupons
                .Where(x => !x.IsRedeemed && x.ExpiryDate > DateTime.UtcNow)
                .Select(x => new CouponDto
                {
                    Id = x.Id,
                    Code = x.Code,
                    Value = x.Value,
                    ExpiryDate = x.ExpiryDate
                })
                .ToListAsync();

            return coupons;
        }
        //public async Task<CouponRedeemResponseDto> RedeemCouponAsync(int userId, CouponRedeemRequestDto request)
        //{
        //    await using var transaction = await _context.Database.BeginTransactionAsync();

        //    try
        //    {
        //        if (string.IsNullOrWhiteSpace(request.IdempotencyKey))
        //        {
        //            request.IdempotencyKey = Guid.NewGuid().ToString();
        //        }
        //        var existingTx = await _context.Transactions
        //            .FirstOrDefaultAsync(x => x.IdempotencyKey == request.IdempotencyKey);

        //        if (existingTx != null)
        //        {
        //            return new CouponRedeemResponseDto
        //            {
        //                RedemptionStatus = RedemptionStatus.AlreadyProcessed,
        //                CouponId = existingTx.CouponId,
        //                UpdatedWalletAmount = await GetWalletBalance(existingTx.UserId)
        //            };
        //        }

        //        var coupon = await _context.Coupons
        //            .FirstOrDefaultAsync(x => x.Code == request.QrCodeValue);

        //        if (coupon == null)
        //            return new CouponRedeemResponseDto { RedemptionStatus = RedemptionStatus.InvalidCoupon };

        //        if (coupon.ExpiryDate < DateTime.UtcNow)
        //            return new CouponRedeemResponseDto { RedemptionStatus = RedemptionStatus.Expired };

        //        if (coupon.IsRedeemed)
        //            return new CouponRedeemResponseDto { RedemptionStatus = RedemptionStatus.AlreadyRedeemed };

        //        var updatedRows = await _context.Database.ExecuteSqlInterpolatedAsync($@"
        //    UPDATE Coupons
        //    SET IsRedeemed = 1
        //    WHERE Id = {coupon.Id} AND IsRedeemed = 0
        //");

        //        if (updatedRows == 0)
        //        {
        //            return new CouponRedeemResponseDto
        //            {
        //                RedemptionStatus = RedemptionStatus.ConcurrencyConflict
        //            };
        //        }

        //        var wallet = await _context.Wallets
        //            .FirstOrDefaultAsync(x => x.UserId == userId);

        //        if (wallet == null)
        //            throw new Exception("Wallet not found");

        //        wallet.Balance += coupon.Value;

        //        var tx = new DbModels.Transaction
        //        {
        //            UserId = userId,
        //            CouponId = coupon.Id,
        //            Amount = coupon.Value,
        //            Status = "Success",
        //            CreatedAt = DateTime.UtcNow,
        //            IdempotencyKey = request.IdempotencyKey
        //        };

        //        _context.Transactions.Add(tx);

        //        await _context.SaveChangesAsync();
        //        await transaction.CommitAsync();

        //        return new CouponRedeemResponseDto
        //        {
        //            RedemptionStatus = RedemptionStatus.Success,
        //            CouponId = coupon.Id,
        //            UpdatedWalletAmount = wallet.Balance,
        //            RedeemedAt = DateTime.UtcNow
        //        };
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        await transaction.RollbackAsync();
        //        return new CouponRedeemResponseDto
        //        {
        //            RedemptionStatus = RedemptionStatus.Error
        //        };
        //    }
        //    catch (Exception)
        //    {
        //        await transaction.RollbackAsync();
        //        return new CouponRedeemResponseDto
        //        {
        //            RedemptionStatus = RedemptionStatus.Error
        //        };
        //    }
        //}

        public async Task<CouponRedeemResponseDto> RedeemCouponAsync(int userId, CouponRedeemRequestDto request)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                if (string.IsNullOrWhiteSpace(request.IdempotencyKey))
                {
                    request.IdempotencyKey = Guid.NewGuid().ToString();
                }

                // 1. Idempotency check
                var existingTx = await _context.Transactions
                    .FirstOrDefaultAsync(x => x.IdempotencyKey == request.IdempotencyKey);

                if (existingTx != null)
                {
                    return new CouponRedeemResponseDto
                    {
                        RedemptionStatus = RedemptionStatus.AlreadyProcessed,
                        CouponId = existingTx.CouponId,
                        UpdatedWalletAmount = await GetWalletBalance(existingTx.UserId),
                        RedeemedAt = existingTx.CreatedAt
                    };
                }

                // 2. Get coupon
                var coupon = await _context.Coupons
                    .FirstOrDefaultAsync(x => x.Code == request.QrCodeValue);

                if (coupon == null)
                {
                    await LogTransaction(userId, 0, 0, request.IdempotencyKey, RedemptionStatus.InvalidCoupon);
                    await transaction.CommitAsync();

                    return new CouponRedeemResponseDto
                    {
                        RedemptionStatus = RedemptionStatus.InvalidCoupon
                    };
                }

                if (coupon.ExpiryDate < DateTime.UtcNow)
                {
                    await LogTransaction(userId, coupon.Id, coupon.Value, request.IdempotencyKey, RedemptionStatus.Expired);
                    await transaction.CommitAsync();

                    return new CouponRedeemResponseDto
                    {
                        RedemptionStatus = RedemptionStatus.Expired
                    };
                }

                if (coupon.IsRedeemed)
                {
                    await LogTransaction(userId, coupon.Id, coupon.Value, request.IdempotencyKey, RedemptionStatus.AlreadyRedeemed);
                    await transaction.CommitAsync();

                    return new CouponRedeemResponseDto
                    {
                        RedemptionStatus = RedemptionStatus.AlreadyRedeemed
                    };
                }

                // 3. Concurrency-safe coupon update
                var updatedRows = await _context.Database.ExecuteSqlInterpolatedAsync($@"
                    UPDATE Coupons
                    SET IsRedeemed = 1
                    WHERE Id = {coupon.Id} AND IsRedeemed = 0
                ");

                if (updatedRows == 0)
                {
                    await LogTransaction(userId, coupon.Id, coupon.Value, request.IdempotencyKey, RedemptionStatus.ConcurrencyConflict);
                    await transaction.CommitAsync();

                    return new CouponRedeemResponseDto
                    {
                        RedemptionStatus = RedemptionStatus.ConcurrencyConflict
                    };
                }

                // 4. Wallet update (safe)
                var wallet = await _context.Wallets
                    .FirstOrDefaultAsync(x => x.UserId == userId);

                if (wallet == null)
                {
                    await LogTransaction(userId, coupon.Id, coupon.Value, request.IdempotencyKey, RedemptionStatus.Failed);
                    throw new Exception("Wallet not found");
                }

                wallet.Balance += coupon.Value;

                // 5. Final success transaction
                await LogTransaction(userId, coupon.Id, coupon.Value, request.IdempotencyKey, RedemptionStatus.Success);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return new CouponRedeemResponseDto
                {
                    RedemptionStatus = RedemptionStatus.Success,
                    CouponId = coupon.Id,
                    UpdatedWalletAmount = wallet.Balance,
                    RedeemedAt = DateTime.UtcNow
                };
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();

                return new CouponRedeemResponseDto
                {
                    RedemptionStatus = RedemptionStatus.Error
                };
            }
        }

        private async Task<decimal> GetWalletBalance(int userId)
        {
            return await _context.Wallets
                .Where(w => w.UserId == userId)
                .Select(w => w.Balance)
                .FirstOrDefaultAsync();
        }

        private async Task LogTransaction(
            int userId,
            int couponId,
            decimal amount,
            string idempotencyKey,
            RedemptionStatus status)
        {
            var tx = new DbModels.Transaction
            {
                UserId = userId,
                CouponId = couponId,
                Amount = amount,
                Status = status.ToString(),
                CreatedAt = DateTime.UtcNow,
                IdempotencyKey = idempotencyKey
            };

            _context.Transactions.Add(tx);
            await _context.SaveChangesAsync();
        }
    }
}
