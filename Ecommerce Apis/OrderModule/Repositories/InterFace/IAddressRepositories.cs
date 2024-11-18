using Ecommerce_Apis.OrderModule.DTOs;
using Ecommerce_Apis.OrderModule.Models;
using Ecommerce_Apis.ProductModule.DTOs;

namespace Ecommerce_Apis.OrderModule.Repositories.InterFace
{
    public interface IAddressRepositories


    {
         Task<bool> AddAddress(AddAddressRequest request);
         Task<bool> DeleteAddress(int id);
        Task<bool> UpdateAddress(Address request);
        Task<List<Address>> GetAddress(string userid);
    }
}
