using Microsoft.EntityFrameworkCore;
using Ecommerce_Apis.BannerModule.Models;
using Ecommerce_Apis.CartModule.DTOs;
using Ecommerce_Apis.CartModule.Repositories.InterFace;
using Ecommerce_Apis.Data;
using System.Reflection;

namespace Ecommerce_Apis.BannerModule.Repositories
{
    public class BannerRepository : IBannerRepository
    {
        private readonly ApplicationDbContext _context;

        public BannerRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> CreateBannerAsync(BannerRequestDB request)
        {
            var banner = new Banner
            {
                Image = request.Image,

                Description = request.Description,Title = request.Title,
                Link = request.Link,
                CouponId = request.CouponId
            };

            _context.BannersData.Add(banner);
            await _context.SaveChangesAsync();
            return banner.Id;
        }

        public async Task<IEnumerable<BannerDTO>> GetAllBannersAsync()
        {
            return await _context.BannersData
                .Select(b => new BannerDTO
                {
                    Id = b.Id,
                   Title = b.Title,
                    Description = b.Description,
                    Link = b.Link,
                    CouponId = b.CouponId,
                    Image = b.Image
                })
                .ToListAsync();
        }

        public async Task<bool> DeleteBanner(int id)
        {
            var banner = await _context.BannersData.FindAsync(id);
            if (banner == null)
                return false;

            _context.BannersData.Remove(banner);
            return await _context.SaveChangesAsync() > 0;
        }
    
    }
}
