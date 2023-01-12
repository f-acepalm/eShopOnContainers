namespace Microsoft.eShopOnContainers.Services.Ordering.API.Application.IntegrationEvents.Events;

public record OrderStatusChangedToStockConfirmedIntegrationEvent : IntegrationEvent
{
    public int OrderId { get; }
    public string OrderStatus { get; }
    public string BuyerName { get; }
    public string BuyerId { get; }
    public decimal Price { get; }

    public OrderStatusChangedToStockConfirmedIntegrationEvent(
        int orderId,
        string orderStatus, 
        string buyerName, 
        string buyerId, 
        decimal price)
    {
        OrderId = orderId;
        OrderStatus = orderStatus;
        BuyerName = buyerName;
        BuyerId = buyerId;
        this.Price = price;
    }
}
