using System;
using System.Collections.Generic;

namespace QRCouponSystem.DbModels;

public partial class Campaign
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public DateTime? ExpiryDate { get; set; }

    public virtual ICollection<Coupon> Coupons { get; set; } = new List<Coupon>();
}
