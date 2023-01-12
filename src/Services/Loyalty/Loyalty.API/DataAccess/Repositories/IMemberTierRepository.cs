using Loyalty.API.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Loyalty.API.DataAccess.Repositories;

public interface IMemberTierRepository : IRepository
{
    Task<MemberTier?> GetMemberTierAsync(Guid userId);
    Task<IEnumerable<MemberTier>> GetMemberTiersAsync();
}
