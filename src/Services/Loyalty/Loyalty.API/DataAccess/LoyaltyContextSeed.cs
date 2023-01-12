using Loyalty.API.DataAccess.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Loyalty.API.DataAccess;

public class LoyaltyContextSeed
{
    public async Task SeedAsync(LoyaltyContext context, IWebHostEnvironment env, ILogger<LoyaltyContextSeed> logger)
    {
        AsyncRetryPolicy policy = CreatePolicy(logger, nameof(LoyaltyContextSeed));
        await policy.ExecuteAsync(async () =>
        {
            if (!context.MemberTiers.Any())
            {
                await context.MemberTiers.AddRangeAsync(GetMemberTiers());
                await context.SaveChangesAsync(); 
            }
        });
    }

    private MemberTier[] GetMemberTiers()
    {
        return new[]
        {
            new MemberTier { Name = "newcomer", Threshold = 0, Discount = 0 },
            new MemberTier { Name = "advanced", Threshold = 20, Discount = 10 },
            new MemberTier { Name = "expert", Threshold = 50, Discount = 20 },
        };
    }

    private AsyncRetryPolicy CreatePolicy(ILogger<LoyaltyContextSeed> logger, string prefix, int retries = 3)
    {
        return Policy.Handle<SqlException>().
            WaitAndRetryAsync(
                retryCount: retries,
                sleepDurationProvider: retry => TimeSpan.FromSeconds(5),
                onRetry: (exception, timeSpan, retry, ctx) =>
                {
                    logger.LogWarning(exception, "[{prefix}] Exception {ExceptionType} with message {Message} detected on attempt {retry} of {retries}", prefix, exception.GetType().Name, exception.Message, retry, retries);
                }
            );
    }
}
