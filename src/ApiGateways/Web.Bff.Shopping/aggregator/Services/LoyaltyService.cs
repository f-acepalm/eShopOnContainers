namespace Microsoft.eShopOnContainers.Web.Shopping.HttpAggregator.Services;

public class LoyaltyService : ILoyaltyService
{
    public readonly HttpClient _httpClient;
    private readonly UrlsConfig _urls;
    private readonly ILogger<LoyaltyService> _logger;

    public LoyaltyService(HttpClient httpClient, IOptions<UrlsConfig> config, ILogger<LoyaltyService> logger)
    {
        _urls = config.Value;
        _httpClient = httpClient;
        _logger = logger;
    }


    public async Task<HttpResponseMessage> GetMemberTiersAsync()
    {
        _logger.LogInformation("Call loyalty api GetMemberTiersAsync");

        var url = new Uri($"{_urls.Loyalty}/api/v1/loyalty");
        HttpResponseMessage response = await _httpClient.GetAsync(url);

        _logger.LogInformation("Loyalty api response: {@response}", response);

        return response;
    }
}
