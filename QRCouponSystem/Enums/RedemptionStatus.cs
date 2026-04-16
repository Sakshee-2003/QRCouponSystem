namespace QRCouponSystem.Enums
{
    public enum RedemptionStatus
    {
        Success,
        Failed,
        InvalidCoupon,
        Expired,
        AlreadyRedeemed,
        ConcurrencyConflict,
        AlreadyProcessed,
        Error
    }
}
