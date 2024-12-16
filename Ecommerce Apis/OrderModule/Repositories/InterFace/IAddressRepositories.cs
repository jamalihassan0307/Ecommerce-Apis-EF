using Ecommerce_Apis.OrderModule.DTOs;
using Ecommerce_Apis.OrderModule.Models;
using Ecommerce_Apis.ProductModule.DTOs;

namespace Ecommerce_Apis.OrderModule.Repositories.InterFace
{
    public interface IAddressRepositories
    {
        Task<bool> AddAddress(AddAddressRequest request);
        Task<List<Address>> GetAddress(string userId);
        Task<Address> GetAddressById(int id);
        Task<bool> UpdateAddress(UpdateAddressDTO request);
        Task<bool> DeleteAddress(int id);
    }
}
