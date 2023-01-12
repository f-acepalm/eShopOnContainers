using Newtonsoft.Json;

namespace Ordering.API.Application.IntegrationEvents.Events;

public record OrderCouponConfirmedIntegrationEvent : IntegrationEvent
{
    [JsonProperty]
    public int OrderId { get; private set; }

    [JsonProperty]
    public int Discount { get; private set; }

    public OrderCouponConfirmedIntegrationEvent(int orderId, int discount)
    {
        OrderId = orderId;
        Discount = discount;
    }
}