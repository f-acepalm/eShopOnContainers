using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;

namespace Loyalty.API.IntegrationEvents.Events;

public record OrderPaymentFailedIntegrationEvent : IntegrationEvent
{
    public int OrderId { get; }

    public OrderPaymentFailedIntegrationEvent(int orderId) => OrderId = orderId;
}
