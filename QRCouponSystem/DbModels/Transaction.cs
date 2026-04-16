using System;
using System.Collections.Generic;

namespace QRCouponSystem.DbModels;

public partial class Transaction
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int CouponId { get; set; }

    public decimal Amount { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? IdempotencyKey { get; set; }

    public virtual User? User { get; set; }
}
