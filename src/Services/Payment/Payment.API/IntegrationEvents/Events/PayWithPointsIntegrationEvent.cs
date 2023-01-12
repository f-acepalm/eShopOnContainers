namespace Payment.API.IntegrationEvents.Events;

public record PayWithPointsIntegrationEvent(int OrderId, string BuyerId, decimal Price) : IntegrationEvent;