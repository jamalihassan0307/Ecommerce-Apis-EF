using Ecommerce_Apis.CartModule.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce_Apis.CartModule.Repositories.InterFace
{
    public interface IBannerRepository
    {
        Task<int> CreateBannerAsync(BannerRequestDB imagePath);
        Task<IEnumerable<BannerDTO>> GetAllBannersAsync();
         Task<bool> DeleteBanner(int Id);
    }
}
