﻿using Ecommerce_Apis.Data;
using Microsoft.EntityFrameworkCore;
using Ecommerce_Apis.ProductModule.DTOs;
using Ecommerce_Apis.ProductModule.Models;
using Ecommerce_Apis.ProductModule.Repositories.InterFace;

namespace Ecommerce_Apis.ProductModule.Repositories
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly ApplicationDbContext _context;

        public ReviewRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AddReviewAsync(AddReviewRequestDTO request, string userId)
        {
            try
            {
                var product = await _context.Products.FindAsync(request.ProductId);
                if (product == null) return false;

                var review = new Review
                {
                    ProductId = request.ProductId,
                    UserId = userId,
                    Message = request.Message,
                    Points = request.Points,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Reviews.Add(review);
                return await _context.SaveChangesAsync() > 0;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<Review>> GetReviews(int productId)
        {
            return await _context.Reviews
                .Include(r => r.User)
                .Include(r => r.Product)
                .Where(r => r.ProductId == productId)
                .Select(r => new Review
                {
                    Id = r.Id,
                    ProductId = r.ProductId,
                    UserId = r.UserId,
                    Message = r.Message,
                    Points = r.Points,
                    CreatedAt = r.CreatedAt
                })
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<bool> DeleteReview(int id)
        {
            try
            {
                var review = await _context.Reviews.FindAsync(id);
                if (review == null) return false;

                _context.Reviews.Remove(review);
                return await _context.SaveChangesAsync() > 0;
            }
            catch
            {
                return false;
            }
        }
    }
}
