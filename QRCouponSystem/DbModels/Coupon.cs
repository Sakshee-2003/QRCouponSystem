using System;
using System.Collections.Generic;

namespace QRCouponSystem.DbModels;

public partial class Coupon
{
    public int Id { get; set; }

    public string? Code { get; set; }

    public int? CampaignId { get; set; }

    public bool IsRedeemed { get; set; }

    public DateTime? ExpiryDate { get; set; }

    public decimal Value { get; set; }

    public byte[] RowVersion { get; set; } = null!;

    public virtual Campaign? Campaign { get; set; }
}
