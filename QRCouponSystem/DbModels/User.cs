using System;
using System.Collections.Generic;

namespace QRCouponSystem.DbModels;

public partial class User
{
    public int Id { get; set; }

    public string? Username { get; set; }

    public string? PasswordHash { get; set; }

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

    public virtual Wallet? Wallet { get; set; }
}
