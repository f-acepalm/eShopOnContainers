using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;

namespace Loyalty.API.IntegrationEvents.Events;

public record PayWithPointsIntegrationEvent(int OrderId, string BuyerId, decimal Price) : IntegrationEvent;
