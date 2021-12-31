using AutoMapper;
using EventBus.Messages.Events;
using Ordering.Application.Features.Orders.Commands.ChreckoutOrder;

namespace Ordering.API.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CheckoutOrderCommand, BasketCheckoutEvent>().ReverseMap();
        }
        
    }
}
