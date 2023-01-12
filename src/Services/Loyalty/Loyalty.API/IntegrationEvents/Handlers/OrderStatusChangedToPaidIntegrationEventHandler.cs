using Loyalty.API.DataAccess.Entities;
using Loyalty.API.DataAccess.Repositories;
using Loyalty.API.IntegrationEvents.Events;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;
using System;
using System.Threading.Tasks;

namespace Loyalty.API.IntegrationEvents.Handlers;

public class OrderStatusChangedToPaidIntegrationEventHandler : IIntegrationEventHandler<OrderStatusChangedToPaidIntegrationEvent>
{
    private readonly ILoyaltyMemberRepository _loyaltyMemberRepository;

    public OrderStatusChangedToPaidIntegrationEventHandler(ILoyaltyMemberRepository loyaltyMemberRepository)
    {
        _loyaltyMemberRepository = loyaltyMemberRepository;
    }
    public async Task Handle(OrderStatusChangedToPaidIntegrationEvent @event)
    {
        var userId = Guid.Parse(@event.BuyerId);
        var loyaltyMember = await _loyaltyMemberRepository.GetLoyaltyMemberAsync(userId);
        var memberIsNew = loyaltyMember is null;
        if (memberIsNew)
        {
            loyaltyMember = new LoyaltyMember()
            {
                Id = userId,
            };
        }

        loyaltyMember!.Points += CalculatePoints(@event.TotalPrice);
        loyaltyMember!.TotalSpent += @event.TotalPrice;

        if (memberIsNew)
        {
            _loyaltyMemberRepository.AddLoyaltyMember(loyaltyMember);
        }
        else
        {
            _loyaltyMemberRepository.UpdateLoyaltyMember(loyaltyMember);
        }

        await _loyaltyMemberRepository.UnitOfWork.SaveChangesAsync();
    }

    private int CalculatePoints(decimal totalPrice)
    {
        return (int)Math.Round(totalPrice / 10);
    }
}
