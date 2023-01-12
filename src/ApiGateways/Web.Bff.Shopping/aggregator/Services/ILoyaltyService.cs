namespace Microsoft.eShopOnContainers.Web.Shopping.HttpAggregator.Services;

public interface ILoyaltyService
{
    Task<HttpResponseMessage> GetMemberTiersAsync();
}