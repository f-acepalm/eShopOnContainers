namespace Microsoft.eShopOnContainers.Payment.API.IntegrationEvents.Events;
    
public record OrderStatusChangedToStockConfirmedIntegrationEvent : IntegrationEvent
{
    public int OrderId { get; }
    public string BuyerId { get; }
    public decimal Price { get; }

    public OrderStatusChangedToStockConfirmedIntegrationEvent(int orderId, string buyerId, decimal price)
    {
        OrderId = orderId;
        BuyerId = buyerId;
        Price = price;
    }
}
