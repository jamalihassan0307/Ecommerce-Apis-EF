using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ecommerce_Apis.OrderModule.DTOs;
using Ecommerce_Apis.OrderModule.Repositories.InterFace;
using Ecommerce_Apis.ResponseMessage;
using Ecommerce_Apis.Utills;

namespace Ecommerce_Apis.OrderModule.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]/[action]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepositories _orderRepositories;
        private readonly IMapper _mapper;

        public OrderController(IOrderRepositories orderRepositories, IMapper mapper)
        {
            _orderRepositories = orderRepositories;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
        {
            var userId = Request.GetUser();
            ResponseDTO response = new();

            try
            {
                var data = await _orderRepositories.CreateOrder(request, userId);
                response.Data = data;
                response.Message = data == null ? MessageDisplay.notFound : MessageDisplay.Orderadd;
                return Ok(response);
            }
            catch (Exception)
            {
                response.Message = MessageDisplay.error;
                response.Status = 404;
                response.IsSuccess = false;
                return BadRequest(response);
            }
        }

        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetOrderById(int orderId)
        {
            ResponseDTO response = new();
            var role = Request.GetRole();
            if (role == "Admin")
            {
                try
                {
                    var data = await _orderRepositories.GetOrderById(orderId, _mapper);
                    response.Data = data;
                    response.Message = data == null ? MessageDisplay.notFound : MessageDisplay.Orderget;
                    return Ok(response);
                }
                catch (Exception)
                {
                    response.Message = MessageDisplay.error; response.Status = 404;
                    response.IsSuccess = false;
                    return BadRequest(response);
                }
            }
            else
            {
                response.Message = MessageDisplay.auth;
                response.Status = 401;
                response.IsSuccess = false;
                return Unauthorized(response);
            }
        }

        [HttpPut("{orderId}")]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, string status)
        {
            ResponseDTO response = new();
            var role = Request.GetRole();
            if (role == "Admin")
            {
                try
                {
                    if (await _orderRepositories.UpdateOrderStatus(orderId, status))
                    {
                        response.Message = MessageDisplay.Orderupdate;
                        return Ok(response);
                    }
                    else
                    {
                        response.Message = MessageDisplay.Orderupdateerror;
                        response.Status = 404;
                        response.IsSuccess = false;
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
                response.Message = MessageDisplay.auth; response.Status = 401;
                response.IsSuccess = false;
                return Unauthorized(response);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetOrders()
        {
            var userId = Request.GetUser();
            ResponseDTO response = new();
            try
            {
                var data = await _orderRepositories.GetOrdersByUserId(userId);
                response.Message = data == null ? MessageDisplay.notFound : MessageDisplay.Orderget;
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

        [HttpPut("{orderId}")]
        public async Task<IActionResult> CancelOrder(int orderId)
        {
            ResponseDTO response = new();
            try
            {
                var data = await _orderRepositories.CancelOrder(orderId);
                response.Message = data == null ? MessageDisplay.notFound : MessageDisplay.Orderupdate;
                response.Data = data;
                return Ok(response);
            }
            catch (Exception)
            {
                response.Message = MessageDisplay.error;
                response.Status = 404;
                response.IsSuccess = false;
                return BadRequest(response);
            }
        }
        [HttpPut("{orderId}")]
        public async Task<IActionResult> DeliveredlOrder(int orderId)
        {
            ResponseDTO response = new();
            try
            {
                var data = await _orderRepositories.DeliveredlOrder(orderId);
                response.Message = data == null ? MessageDisplay.notFound : MessageDisplay.Orderupdate;
                response.Data = data;
                return Ok(response);
            }
            catch (Exception)
            {
                response.Message = MessageDisplay.error;
                response.Status = 404;
                response.IsSuccess = false;
                return BadRequest(response);
            }
        }

        [HttpDelete("{orderId}")]
        public async Task<IActionResult> DeleteOrder(int orderId)
        {

            ResponseDTO response = new();
            try
            {

                var data = await _orderRepositories.DeleteOrder(orderId);
                response.Message = data == null ? MessageDisplay.notFound : MessageDisplay.Orderdelete;
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

        
        
        [HttpGet("all")]
        public async Task<IActionResult> GetAllOrders()
        {
            ResponseDTO response = new();
            var role = Request.GetRole();
            if (role == "Admin")
            {
                try
                {
                    var data = await _orderRepositories.GetAllOrders(_mapper);
                    response.Data = data;
                    response.Message = data == null || !data.Any() ? MessageDisplay.notFound : MessageDisplay.Orderget;
                    return Ok(response);
                }
                catch (Exception)
                {
                    response.Message = MessageDisplay.error; response.Status = 404;
                    response.IsSuccess = false;
                    return BadRequest(response);
                }
            }
            else
            {
                response.Message = MessageDisplay.auth;
                response.Status = 401;
                response.IsSuccess = false;
                return Unauthorized(response);
            }
        }

     }
}
