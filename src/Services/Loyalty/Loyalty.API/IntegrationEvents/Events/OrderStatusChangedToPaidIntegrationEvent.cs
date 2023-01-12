using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;

namespace Loyalty.API.IntegrationEvents.Events;

public record OrderStatusChangedToPaidIntegrationEvent : IntegrationEvent
{
    public string BuyerId { get; }
    public decimal TotalPrice { get; }

    public OrderStatusChangedToPaidIntegrationEvent(
        string buyerId,
        decimal totalPrice)
    {
        BuyerId = buyerId;
        TotalPrice = totalPrice;
    }
}
