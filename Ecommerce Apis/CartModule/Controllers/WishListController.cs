using AutoMapper;
using Ecommerce_Apis.CartModule.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ecommerce_Apis.Utills;    
using Ecommerce_Apis.CartModule.Repositories.InterFace;
using Ecommerce_Apis.ResponseMessage;

namespace Ecommerce_Apis.CartModule.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Authorize]
    public class WishListController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IWishListRepositories _wishListRepositories;

        public WishListController(IMapper mapper, IWishListRepositories wishListRepositories)
        {
            _mapper = mapper;
            _wishListRepositories = wishListRepositories;
        }

        [HttpPost]
        public async Task<IActionResult> AddWishList([FromBody] AddToCartRequestDTO request)
        {
            var userId = Request.GetUser();
            ResponseDTO response = new();

            try
            {
                if (await _wishListRepositories.CreateWishList(request, userId))
                {
                    response.Message = MessageDisplay.Wishlistadd;
                    return Ok(response);
                }
                else
                {
                    response.Message = MessageDisplay.Wishlisterror;
                    response.Status = 404;
                    response.IsSuccess = false;
                    return BadRequest(response);
                }
            }
            catch (Exception)
            {
                response.Message = MessageDisplay.error;
                response.Status = 404;
                response.IsSuccess = false;
                return BadRequest(response);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetUserWishList()
        {
            var userId = Request.GetUser();
            ResponseDTO response = new();

            try
            {
                var data = await _wishListRepositories.GetUserWishList(userId);
                response.Message = data == null || !data.Any() ? MessageDisplay.notFound : MessageDisplay.Wishlistget;
                response.Data = data;
                return Ok(response);
            }
            catch (Exception)
            {
                response.Message = MessageDisplay.error; response.Status = 404;
                response.IsSuccess = false;
                return BadRequest(response);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetWishListById(int id)
        {
            var userId = Request.GetUser();
            ResponseDTO response = new();

            try
            {
                var data = await _wishListRepositories.GetWishListById(userId, id);
                response.Message = data == null ? MessageDisplay.notFound : MessageDisplay.Wishlistget;
                response.Data = data;
                return Ok(response);
            }
            catch (Exception)
            {
                response.Message = MessageDisplay.error; response.Status = 404;
                response.IsSuccess = false;
                return BadRequest(response);
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateWishListItem(updateItem model)
        {
            ResponseDTO response = new();
            try
            {
                if (await _wishListRepositories.UpdateWishLists(model))
                {
                    response.Message = MessageDisplay.Wishlistupdate;

                    return Ok(response);
                }
                else
                {
                    response.Message = MessageDisplay.Wishlistupdateerror;
                    response.Status = 404;
                    response.IsSuccess = false;
                    return BadRequest(response);
                }
            }
            catch (Exception)
            {
                response.Message = MessageDisplay.Wishlisterror; response.Status = 404;
                response.IsSuccess = false;
                return BadRequest(response);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWishListItem(int id)
        {
            ResponseDTO response = new();
            try
            {
                if (await _wishListRepositories.DeleteWishListItem(id))
                {
                    response.Message = MessageDisplay.Wishlistdelete;
                    return Ok(response);
                }
                else
                {
                    response.Message = MessageDisplay.Wishlistdeleteerror;
                    response.Status = 404;
                    response.IsSuccess = false;
                    return BadRequest(response);
                }
            }
            catch (Exception)
            {
                response.Message = MessageDisplay.error; response.Status = 404;
                response.IsSuccess = false;
                return BadRequest(response);
            }
        }

       
    }
}
