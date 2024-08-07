using AutoMapper;
using Talabat.APIs.Dtos;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Order_Aggregation;

//using Talabat.Core.Entities.Identity;

namespace Talabat.APIs.Helpers
{
    public class MappingProfiles:Profile
    {
        public MappingProfiles()
        {
            CreateMap<Product, ProductToReturnDto>()
                        .ForMember(PD => PD.ProductType, O => O.MapFrom(P => P.ProductType.Name))
                        .ForMember(PD => PD.ProductBrand, O => O.MapFrom(P => P.ProductBrand.Name))
                        .ForMember(PD => PD.PictureUrl, O => O.MapFrom<ProductPictureUrlResolver>());

           // CreateMap<Address, AddressDto>().ReverseMap();
            //CreateMap<AddressDto, Address>().ReverseMap(); 

            CreateMap<CustomerBasketDto, CustomerBasket>();
            CreateMap<BasketItemDto, BasketItem>();

            CreateMap<Talabat.Core.Entities.Identity.Address, AddressDto>().ReverseMap();
            CreateMap<AddressDto, Talabat.Core.Entities.Order_Aggregation.Address>();

            // =======Order======//
            CreateMap<Order, OrderToReturnDto>()
                    .ForMember(D => D.DeliveryMethod, O => O.MapFrom(m => m.DeliveryMethod.ShortName))
                    .ForMember(D => D.DeliveryMethodCost, O => O.MapFrom(m => m.DeliveryMethod.Cost));

            CreateMap<OrderItem, OrderItemDto>()
                    .ForMember(D => D.ProductId, O => O.MapFrom(m => m.Product.ProductId))
                    .ForMember(D => D.ProductName, O => O.MapFrom(m => m.Product.ProductName))
                    .ForMember(D => D.PictureUrl, O => O.MapFrom(m => m.Product.PictureUrl))
                    .ForMember(D => D.PictureUrl, O => O.MapFrom<OrderPictureUrlResolver>());

        }
    }
}
