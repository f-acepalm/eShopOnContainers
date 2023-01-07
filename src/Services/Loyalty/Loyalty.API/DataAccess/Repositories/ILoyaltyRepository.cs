using Loyalty.API.DataAccess.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Loyalty.API.DataAccess.Repositories;

public interface ILoyaltyRepository
{
    Task<IEnumerable<MemberTier>> GetMemberTiers();
}
