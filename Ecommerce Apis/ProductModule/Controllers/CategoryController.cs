using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Ecommerce_Apis.ProductModule.DTOs;
using Ecommerce_Apis.ResponseMessage;
using Ecommerce_Apis.ProductModule.Repositories.InterFace;
using Ecommerce_Apis.Utills;
using Microsoft.AspNetCore.Authorization;

namespace Ecommerce_Apis.ProductModule.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IWebHostEnvironment _environment;
        private readonly IMapper _mapper;

        public CategoryController(ICategoryRepository categoryRepository, 
            IWebHostEnvironment webHostEnvironment,
            IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _environment = webHostEnvironment;
            _mapper = mapper;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddCategory([FromForm] AddCategoryRequest request)
        {
            ResponseDTO response = new();
            var role = Request.GetRole();
            if (role == "Admin")
            {
                try
                {
                    if (request.ImagePath != null)
                    {
                        string imagePath = await FileManage.UploadAsync(request.ImagePath, _environment);
                        var addCategoryRequest = new CategoryRequest 
                        { 
                            Name = request.Name, 
                            ParentId = request.ParentId, 
                            ImagePath = imagePath
                        };

                        if (await _categoryRepository.AddCategory(addCategoryRequest))
                        {
                            response.Message = MessageDisplay.categoryadd;
                            return Ok(response);
                        }
                        else
                        {
                            response.Message = MessageDisplay.categoryerror;
                            return BadRequest(response);
                        }
                    }
                    else
                    {
                        response.Message = "Image file is required.";
                        return BadRequest(response);
                    }
                }
                catch (Exception ex)
                {
                    response.Message = ex.Message.Contains("foreign key constraint") 
                        ? MessageDisplay.categoryParenterror 
                        : MessageDisplay.error;
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
        [Authorize]
        public async Task<IActionResult> UpdateCategory([FromForm] UpdateCategory request)
        {
            var role = Request.GetRole();
            ResponseDTO response = new();

            if (role == "Admin")
            {
                try
                {
                    var existingCategory = await _categoryRepository.GetCategoryById(request.Id);
                    if (existingCategory == null)
                    {
                        response.Message = "Category not found.";
                        return NotFound(response);
                    }

                    var categoryToUpdate = new UpdateCategoryDTO
                    {
                        Id = request.Id,
                        Name = !string.IsNullOrEmpty(request.Name) ? request.Name : existingCategory.Name,
                        ParentId = request.ParentId,
                        ImagePath = existingCategory.ImagePath // Keep existing image by default
                    };

                    // Only update image if a new one is provided
                    if (request.ImagePath != null)
                    {
                        categoryToUpdate.ImagePath = await FileManage.UploadAsync(request.ImagePath, _environment);
                    }

                    if (await _categoryRepository.UpdateCategory(categoryToUpdate))
                    {
                        response.Message = MessageDisplay.categoryupdate;
                        return Ok(response);
                    }
                    else
                    {
                        response.Message = MessageDisplay.categoryupdateerror;
                        return BadRequest(response);
                    }
                }
                catch (Exception ex)
                {
                    response.Message = ex.Message.Contains("foreign key constraint") 
                        ? MessageDisplay.categoryParenterror 
                        : MessageDisplay.error;
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
        [AllowAnonymous]
        public ActionResult GetAllCategories()
        {
            ResponseDTO response = new();
            try
            {
                var data = _categoryRepository.GetAllCategories();
                response.Data = data;
                response.Message = data == null || !data.Any() 
                    ? MessageDisplay.notFound 
                    : MessageDisplay.categoryget;
                return Ok(response);
            }
            catch (Exception)
            {
                response.Message = MessageDisplay.error;
                return BadRequest(response);
            }
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult> GetCategoryById(int id)
        {
            ResponseDTO response = new();
            try
            {
                var category = await _categoryRepository.GetCategoryById(id);
                if (category == null)
                {
                    response.Message = MessageDisplay.notFound;
                    return NotFound(response);
                }

                response.Data = category;
                response.Message = MessageDisplay.categoryget;
                return Ok(response);
            }
            catch (Exception)
            {
                response.Message = MessageDisplay.error;
                return BadRequest(response);
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            ResponseDTO response = new();
            var role = Request.GetRole();
            if (role == "Admin")
            {
                try
                {
                    if (await _categoryRepository.DeleteCategory(id))
                    {
                        response.Message = MessageDisplay.categoryget;
                        return Ok(response);
                    }
                    else
                    {
                        response.Message = MessageDisplay.categorydeleteerror;
                        return NotFound(response);
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
