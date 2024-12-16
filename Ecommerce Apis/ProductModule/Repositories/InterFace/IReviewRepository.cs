using Ecommerce_Apis.ProductModule.DTOs;
using Ecommerce_Apis.ProductModule.Models;

namespace Ecommerce_Apis.ProductModule.Repositories.InterFace
{
    public interface IReviewRepository
    {
        public Task<bool> AddReviewAsync(AddReviewRequestDTO request,string userId);
        public Task<bool> DeleteReview(int id);
        public Task<List<Review>> GetReviews(int productId);
    }
}
