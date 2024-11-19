using AutoMapper;
using Ecommerce_Apis.OrderModule.DTOs;
using Ecommerce_Apis.OrderModule.Models;

namespace Ecommerce_Apis.OrderModule.Mappings
{
    public class AddressMappingProfile : Profile
    {
        public AddressMappingProfile()
        {
            CreateMap<UpdateAddressDTO, Address>()
                .ForMember(dest => dest.UserId, opt => opt.Ignore());
        }
    }
} 