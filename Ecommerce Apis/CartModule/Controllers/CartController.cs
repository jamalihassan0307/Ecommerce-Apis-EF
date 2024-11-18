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
    public class CartController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ICartRepositories _cartRepositories;

        public CartController(IMapper mapper, ICartRepositories cartRepositories)
        {
            _mapper = mapper;
            _cartRepositories = cartRepositories;   
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(AddToCartRequestDTO request)
        {
            ResponseDTO response = new();
            var userId = Request.GetUser();

            try
            {
                if (await _cartRepositories.CreateCart(request, userId))
                {
                    response.Message = MessageDisplay.cartadd;
                    return Ok(response);
                }
                else
                {
                    response.Message = MessageDisplay.carterror;
                    return BadRequest(response);
                }
            }
            catch (Exception)
            {
                response.Message = MessageDisplay.error;
                return BadRequest(response);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetUserCart()
        {
            var userId = Request.GetUser();
            ResponseDTO response = new();

            try
            {
                var data = await _cartRepositories.GetUserCart(userId);
                response.Message = data == null || !data.Any() ? MessageDisplay.notFound : MessageDisplay.cartget;
                response.Data = data;
                return Ok(response);
            }
            catch (Exception)
            {
                response.Message = MessageDisplay.error;
                return BadRequest(response);
            }
        }

        [HttpGet("{cartId}")]
        public async Task<IActionResult> GetCartById(int cartId)
        {
            var userId = Request.GetUser();
            ResponseDTO response = new();
            var role = Request.GetRole();

            if (role == "Admin")
            {
                try
                {
                    var data = await _cartRepositories.GetCartById(userId, cartId);
                    response.Data = data;
                    response.Message = data == null ? MessageDisplay.notFound : MessageDisplay.cartget;
                    return Ok(response);
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

        [HttpPut]
        public async Task<IActionResult> UpdateCartItems(UpdateCartRequestDTO model)
        {
            ResponseDTO response = new();
            try
            {
                if (await _cartRepositories.UpdateCartItems(model))
                {
                    response.Message = MessageDisplay.cartupdate;
                    return Ok(response);
                }
                else
                {
                    response.Message = MessageDisplay.cartupdateerror;
                    return BadRequest(response);
                }
            }
            catch (Exception)
            {
                response.Message = MessageDisplay.error;
                return BadRequest(response);
            }
        }

        [HttpDelete("{cartId}")]
        public async Task<IActionResult> DeleteCartItem(int cartId)
        {
            ResponseDTO response = new();
            try
            {
                if (await _cartRepositories.DeleteCartItem(cartId))
                {
                    response.Message = MessageDisplay.cartdelete;
                    return Ok(response);
                }
                else
                {
                    response.Message = MessageDisplay.cartdeleteerror;
                    return BadRequest(response);
                }
            }
            catch (Exception)
            {
                response.Message = MessageDisplay.error;
                return BadRequest(response);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteAllUserCart()
        {
            ResponseDTO response = new();
            try
            {
                var userId = Request.GetUser();
                if (await _cartRepositories.DeleteAllUserCart(userId))
                {
                    response.Message = MessageDisplay.cartdelete;
                    return Ok(response);
                }
                else
                {
                    response.Message = MessageDisplay.cartdeleteerror;
                    return BadRequest(response);
                }
            }
            catch (Exception)
            {
                response.Message = MessageDisplay.error;
                return BadRequest(response);
            }
        }
    }
}
