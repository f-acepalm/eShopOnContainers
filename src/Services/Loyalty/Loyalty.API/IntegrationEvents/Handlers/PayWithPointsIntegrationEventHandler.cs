using Loyalty.API.DataAccess.Repositories;
using Loyalty.API.IntegrationEvents.Events;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Loyalty.API.IntegrationEvents.Handlers;

public class PayWithPointsIntegrationEventHandler : IIntegrationEventHandler<PayWithPointsIntegrationEvent>
{
    private readonly IEventBus _eventBus;
    private readonly ILoyaltyMemberRepository _loyaltyMemberRepository;
    private readonly ILogger<PayWithPointsIntegrationEventHandler> _logger;

    public PayWithPointsIntegrationEventHandler(
        IEventBus eventBus,
        ILoyaltyMemberRepository loyaltyMemberRepository,
        ILogger<PayWithPointsIntegrationEventHandler> logger)
    {
        _eventBus = eventBus;
        _loyaltyMemberRepository = loyaltyMemberRepository;
        _logger = logger;
    }

    public async Task Handle(PayWithPointsIntegrationEvent @event)
    {
        _logger.LogInformation("----- Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", @event.Id, Program.AppName, @event);

        var userId = Guid.Parse(@event.BuyerId);
        var loyaltyMember = await _loyaltyMemberRepository.GetLoyaltyMemberAsync(userId);
        var priceInPoints = GetPriceInPoints(@event);
        IntegrationEvent orderPaymentIntegrationEvent;

        if (loyaltyMember is not null && loyaltyMember.Points >= priceInPoints)
        {
            loyaltyMember.Points -= priceInPoints;
            _loyaltyMemberRepository.UpdateLoyaltyMember(loyaltyMember);
            await _loyaltyMemberRepository.UnitOfWork.SaveChangesAsync();

            orderPaymentIntegrationEvent = new OrderPaymentSucceededIntegrationEvent(@event.OrderId);
        }
        else
        {
            orderPaymentIntegrationEvent = new OrderPaymentFailedIntegrationEvent(@event.OrderId);
        }

        _logger.LogInformation("----- Publishing integration event: {IntegrationEventId} from {AppName} - ({@IntegrationEvent})", orderPaymentIntegrationEvent.Id, Program.AppName, orderPaymentIntegrationEvent);

        _eventBus.Publish(orderPaymentIntegrationEvent);
    }

    private static int GetPriceInPoints(PayWithPointsIntegrationEvent @event)
    {
        return (int)Math.Round(@event.Price);
    }
}
