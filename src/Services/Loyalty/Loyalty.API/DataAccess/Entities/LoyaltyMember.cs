using System;

namespace Loyalty.API.DataAccess.Entities;

public class LoyaltyMember
{
    public Guid Id { get; set; }

    public decimal TotalSpent { get; set; }

    public int Points { get; set; }
}
