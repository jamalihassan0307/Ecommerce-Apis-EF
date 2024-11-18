using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ecommerce_Apis.ProductModule.DTOs;
using Ecommerce_Apis.ProductModule.Repositories.InterFace;
using Ecommerce_Apis.ResponseMessage;
using Ecommerce_Apis.Utills;

namespace Ecommerce_Apis.ProductModule.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IMapper _mapper;

        public ReviewController(IMapper mapper, IReviewRepository reviewRepository)
        {
            _mapper = mapper;
            _reviewRepository = reviewRepository;
        }

        [HttpPost]
        public async Task<IActionResult> AddReview([FromBody] AddReviewRequestDTO request)
        {
            var userId = Request.GetUser();
            ResponseDTO response = new();

            try
            {
                if (await _reviewRepository.AddReviewAsync(request, userId))
                {
                    response.Message = MessageDisplay.Reviewadd;
                    return Ok(response);
                }
                else
                {
                    response.Message = MessageDisplay.Reviewerror;
                    return BadRequest(response);
                }
            }
            catch (Exception)
            {
                response.Message = MessageDisplay.error;
                return BadRequest(response);
            }
        }

        [HttpGet("{productId}")]
        public async Task<IActionResult> GetReviews(int productId)
        {
            ResponseDTO response = new();
            try
            {
                var data = await _reviewRepository.GetReviews(productId);
                response.Data = data;
                response.Message = data == null || !data.Any() 
                    ? MessageDisplay.notFound 
                    : MessageDisplay.Reviewget;

                return Ok(response);
            }
            catch (Exception)
            {
                response.Message = MessageDisplay.error;
                return BadRequest(response);
            }
        }

        [HttpDelete("deleteReview/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteReview(int id)
        {
            ResponseDTO response = new();
            var role = Request.GetRole();

            if (role == "Admin")
            {
                try
                {
                    if (await _reviewRepository.DeleteReview(id))
                    {
                        response.Message = MessageDisplay.Reviewdelete;
                        return Ok(response);
                    }
                    else
                    {
                        response.Message = MessageDisplay.Reviewdeleteerror;
                        return BadRequest(response);
                    }
                }
                catch (Exception)
                {
                    response.Message = MessageDisplay.error;
                    return BadRequest(response);
                }
            }
            else
            {
                response.Message = MessageDisplay.auth;
                return Unauthorized(response);
            }
        }
    }
}
