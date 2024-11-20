using AutoMapper;
using Ecommerce_Apis.ProductModule.DTOs;
using Ecommerce_Apis.ProductModule.Models;
namespace Ecommerce_Apis.Utills
{
    public class ModelMapping : Profile
    {
        public ModelMapping()
        {
            CreateMap<UpdateProductRequestDTO, Product>();
        }
    }
}
