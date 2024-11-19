using Microsoft.AspNetCore.Mvc;
using Ecommerce_Apis.CartModule.Repositories.InterFace;
using Ecommerce_Apis.Utills;
using Ecommerce_Apis.CartModule.DTOs;
using Ecommerce_Apis.ResponseMessage;
using Microsoft.AspNetCore.Authorization;

namespace Ecommerce_Apis.CartModule.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class BannerController : ControllerBase
    {
        private readonly IBannerRepository _bannerRepository;
        private readonly IWebHostEnvironment _environment;

        public BannerController(IBannerRepository bannerRepository, IWebHostEnvironment webHostEnvironment)
        {
            _bannerRepository = bannerRepository;
            _environment = webHostEnvironment;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddBanner([FromForm] BannerRequest request)
        {
            ResponseDTO response = new();
            var role = Request.GetRole();

            if (role == "Admin")
            {
                try
                {
                    string imagePath = await FileManage.UploadAsync(request.Image, _environment);
                    var bannerRequest = new BannerRequestDB(request.LinkId, request.Link, imagePath, request.CouponId);
                    int bannerId = await _bannerRepository.CreateBannerAsync(bannerRequest);

                    if (bannerId > 0)
                    {
                        response.Message = MessageDisplay.BannerAdd;
                        response.Data = bannerId;
                        return Ok(response);
                    }

                    response.Message = MessageDisplay.Banneradderror;
                    response.Status = 404;
                    response.IsSuccess = false;
                    return BadRequest(response);
                }
                catch (Exception)
                {
                    response.Message = MessageDisplay.error;
                    response.Status = 400;
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

        [HttpGet]
        [AllowAnonymous
    ]

    public async Task<IActionResult> GetAllBanners()
        {
            ResponseDTO response = new();
            try
            {
                var banners = await _bannerRepository.GetAllBannersAsync();
                response.Data = banners;
                response.Message = banners == null || !banners.Any() ? MessageDisplay.notFound : MessageDisplay.Bannerget;
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

        [HttpDelete("{Id}")]
        [Authorize]
        public async Task<IActionResult> DeleteBanner(int Id)
        {
            ResponseDTO response = new();
            var role = Request.GetRole();
            
            if (role != "Admin")
            {
                response.Message = MessageDisplay.auth;
                return Unauthorized(response);
            }

            try
            {
                if (await _bannerRepository.DeleteBanner(Id))
                {
                    response.Message = MessageDisplay.Bannerdelete;
                    return Ok(response);
                }
                else
                {
                    response.Message = MessageDisplay.Bannerdeleteerror;

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
    }
}
