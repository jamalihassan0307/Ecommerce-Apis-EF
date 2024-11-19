using AutoMapper;
using Ecommerce_Apis.OrderModule.Repositories.InterFace;
using Ecommerce_Apis.ResponseMessage;
using Ecommerce_Apis.UserModule.DTOs;
using Ecommerce_Apis.Utills;
using Microsoft.AspNetCore.Mvc;
using Ecommerce_Apis.OrderModule.DTOs;
using Microsoft.AspNetCore.Authorization;
using Ecommerce_Apis.OrderModule.Models;

namespace Ecommerce_Apis.OrderModule.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class AddressController : ControllerBase
    {
        private readonly IAddressRepositories _addressRepository;
        private readonly IMapper _mapper;

        public AddressController(IAddressRepositories addressRepositories, IMapper mapper)
        {
            _addressRepository = addressRepositories;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> AddAddress([FromBody] AddAddressRequestDTO request)
        {
            var userId = Request.GetUser();
            ResponseDTO response = new();
            try
            {
                AddAddressRequest model = new AddAddressRequest(request, userId);
                if (await _addressRepository.AddAddress(model))
                {
                    response.Message = MessageDisplay.Addressadd;
                    return Ok(response);
                }
                else
                {
                    response.Message = MessageDisplay.Addresserror;
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
        public async Task<IActionResult> GetAddresss()
        {
            ResponseDTO response = new();
            try
            {
                var userId = Request.GetUser();
                var data = await _addressRepository.GetAddress(userId);

                response.Data = data;

                response.Message = data == null || data.FirstOrDefault() == null ? MessageDisplay.notFound : MessageDisplay.Addressget;

                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Message = MessageDisplay.error;
                return BadRequest(response);
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateAddress([FromBody] UpdateAddressDTO request)
        {
            ResponseDTO response = new();
            try
            {

                var userId = Request.GetUser();
                if (await _addressRepository.UpdateAddress(request))
                {
                    response.Message = MessageDisplay.AddressUpdated;
                    return Ok(response);
                }
                else
                {
                    response.Message = MessageDisplay.AddressUpdateError;
                    return BadRequest(response);
                }
            }
            catch (Exception ex)
            {
                response.Message = MessageDisplay.error;
                return BadRequest(response);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAddress(int id)
        {
            ResponseDTO response = new();
            try
            {
                if (await _addressRepository.DeleteAddress(id))
                {
                    response.Message = MessageDisplay.Addressdelete;
                    return Ok(response);
                }
                else
                {
                    response.Message = MessageDisplay.Addressdeleteerror;
                    return BadRequest(response);
                }
            }
            catch (Exception ex)
            {
                response.Message = MessageDisplay.error;
                return BadRequest(response);
            }
        }
    }
}