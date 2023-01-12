using Loyalty.API.DataAccess.Entities;
using System;
using System.Threading.Tasks;

namespace Loyalty.API.DataAccess.Repositories;

public interface ILoyaltyMemberRepository : IRepository
{
    void AddLoyaltyMember(LoyaltyMember entity);

    Task<LoyaltyMember?> GetLoyaltyMemberAsync(Guid userId);

    void UpdateLoyaltyMember(LoyaltyMember entity);
}
