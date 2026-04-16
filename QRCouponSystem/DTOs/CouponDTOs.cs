using QRCouponSystem.Enums;

namespace QRCouponSystem.DTOs
{
    public class CouponRedeemRequestDto
    {
        public string? IdempotencyKey { get; set; }
        public string? QrCodeValue { get; set; }
    }

    public class CouponRedeemResponseDto
    {
        public RedemptionStatus RedemptionStatus { get; set; }
        public int? CouponId { get; set; }
        public decimal? UpdatedWalletAmount { get; set; }
        public DateTime? RedeemedAt { get; set; }
        public string? Message { get; set; }
    }

    public class GenerateCouponDto
    {
        public int Count { get; set; }
        public decimal Value { get; set; }
    }

    public class CouponDto
    {
        public int Id { get; set; }
        public string? Code { get; set; }
        public decimal Value { get; set; }
        public DateTime? ExpiryDate { get; set; }
    }

    public class ReconcileResponseDto
    {
        public int TotalTransactionsChecked { get; set; }
        public int CouponsFixed { get; set; }
        public int WalletsCorrected { get; set; }
        public string? Message { get; set; }
    }
}
