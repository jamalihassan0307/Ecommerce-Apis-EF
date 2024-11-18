﻿using AutoMapper;
using Ecommerce_Apis.OrderModule.DTOs;
using Ecommerce_Apis.OrderModule.Repositories.InterFace;
using Ecommerce_Apis.ResponseMessage;
using Ecommerce_Apis.UserModule.DTOs;
using Ecommerce_Apis.Utills;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce_Apis.OrderModule.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]/[action]")]
    public class DeliveryBoyController : ControllerBase
    {
        private readonly IDeliveryBoyRepositories _deliveryBoyRepositories;
        private readonly IMapper _mapper;

        public DeliveryBoyController(IDeliveryBoyRepositories deliveryBoyRepositories, IMapper mapper)
        {
            _deliveryBoyRepositories = deliveryBoyRepositories;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> AssignDelivery([FromBody] AssignDeliveryRequest request)
        {
            ResponseDTO response = new();
            var role = Request.GetRole();
            if (role == "Admin")
            {
                try
                {
                    var success = await _deliveryBoyRepositories.AssignDelivery(request.DeliveryBoyId.ToString(), request.OrderId);
                    response.Message = success ? MessageDisplay.OrderAssigned : MessageDisplay.OrderAssignFailed;
                    return Ok(response);
                }
                catch (Exception ex)
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

        [HttpGet]
        public async Task<IActionResult> GetAssignedOrders()
        {
            ResponseDTO response = new();
            var role = Request.GetRole();
            var id = Request.GetUser();
                if (role == "DeliveryBoy")
                {
            try
            {
                
                    var orders = await _deliveryBoyRepositories.GetAssignedOrders(id);
                    response.Data = orders;
                    response.Message = orders == null || orders.FirstOrDefault()==null ? MessageDisplay.notFound : MessageDisplay.OrdersRetrieved;
                    return Ok(response);
                }
            catch (Exception ex)
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

        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetOrderDetails(int orderId)
        {
            ResponseDTO response = new();
            var role = Request.GetRole();
            if (role == "DeliveryBoy")
            {
                try
            {
                var orderDetails = await _deliveryBoyRepositories.GetOrderDetails(orderId);
                response.Data = orderDetails;
                response.Message = orderDetails == null ? MessageDisplay.notFound : MessageDisplay.OrderRetrieved;
                return Ok(response);
            }
            catch (Exception ex)
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

        [HttpPut("{orderId}/{status}")]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, string status)
        {
            ResponseDTO response = new();
            var role = Request.GetRole();
            if (role == "DeliveryBoy")
            {
                try
            {
                var success = await _deliveryBoyRepositories.UpdateOrderStatus(orderId, status);
                response.Message = success ? MessageDisplay.OrderUpdated : MessageDisplay.OrderUpdateFailed;
                return Ok(response);
            }
            catch (Exception ex)
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

        [HttpPut("{orderId}")]
        public async Task<IActionResult> MarkOrderAsDelivered(int orderId)
        {

            ResponseDTO response = new();
            var role = Request.GetRole();
            if (role == "DeliveryBoy")
            {
                try
            {
                var success = await _deliveryBoyRepositories.MarkOrderAsDelivered(orderId);
                response.Message = success ? MessageDisplay.OrderMarkedDelivered : MessageDisplay.OrderMarkFailed;
                return Ok(response);
            }
            catch (Exception ex)
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
