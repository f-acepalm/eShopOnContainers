namespace Microsoft.eShopOnContainers.Services.Ordering.API.Application.IntegrationEvents.Events;

public record OrderStatusChangedToPaidIntegrationEvent : IntegrationEvent
{
    public int OrderId { get; }
    public string OrderStatus { get; }
    public string BuyerName { get; }
    public IEnumerable<OrderStockItem> OrderStockItems { get; }
    public string BuyerId { get; }
    public decimal TotalPrice { get; }

    public OrderStatusChangedToPaidIntegrationEvent(int orderId,
        string orderStatus,
        string buyerName,
        IEnumerable<OrderStockItem> orderStockItems,
        string buyerId,
        decimal totalPrice)
    {
        OrderId = orderId;
        OrderStockItems = orderStockItems;
        OrderStatus = orderStatus;
        BuyerName = buyerName;
        BuyerId = buyerId;
        TotalPrice = totalPrice;
    }
}

