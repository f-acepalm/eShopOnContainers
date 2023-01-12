using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;

namespace Loyalty.API.IntegrationEvents.Events;

public record OrderPaymentSucceededIntegrationEvent : IntegrationEvent
{
    public int OrderId { get; }

    public OrderPaymentSucceededIntegrationEvent(int orderId) => OrderId = orderId;
}
