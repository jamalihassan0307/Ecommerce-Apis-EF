using AutoMapper;
using Ecommerce_Apis.ResponseMessage;
using Ecommerce_Apis.UserModule.DTOs;
using Ecommerce_Apis.UserModule.Repositories.InterFace;
using Ecommerce_Apis.Utills;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce_Apis.UserModule.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IWebHostEnvironment _environment;
        private readonly ITokenHelper _tokenHelper;
        private readonly IMapper _mapper;

        public UsersController(IWebHostEnvironment environment, ITokenHelper tokenHelper, IMapper mapper, IUserRepository userRepository)
        {
            _environment = environment;
            _tokenHelper = tokenHelper;
            _mapper = mapper;
            _userRepository = userRepository;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> SignupAsync([FromForm] AddUserRequest request)
        {
            ResponseDTO response = new();
            try
            {
                var addUserRequest = new GetUserResponse
                {
                    FullName = request.FullName,
                    Email = request.Email,
                    PhoneNumber = request.PhoneNumber,
                    RoleId = request.RoleId
                };

                addUserRequest.PasswordHash = EncryptionDecryption.Encrypt(request.Password);
                addUserRequest.Image = await FileManage.UploadAsync(request.Image, _environment);



                string id = await _userRepository.Signup(addUserRequest);
                if (id !="")
                {
                    var token = _tokenHelper.GenerateToken(id, "Customer");
                    response.Data = token;
                    response.Message = "Customer registered successfully.";
                    return Ok(response);
                }
                else
                {
                    response.Message = MessageDisplay.error;
                    response.Status = 404;
                    response.IsSuccess = false;
                    return BadRequest(response);
                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message.Contains("Duplicate entry") 
                    ? MessageDisplay.emailduplicated 
                    : MessageDisplay.error; 
                response.Status = 404;
                response.IsSuccess = false;
                return BadRequest(response);
            }
        }

        [HttpPost]
        public async Task<IActionResult> SignupByAdminAsync([FromForm] AddUserRequestRole request)
        {
            ResponseDTO response = new();
            var role = Request.GetRole();
            if (role == "Admin")
            {
                try
                {
                    GetUserResponse addUserRequest = _mapper.Map<AddUserRequestRole, GetUserResponse>(request);
                    addUserRequest.PasswordHash = EncryptionDecryption.Encrypt(request.Password);
                    addUserRequest.Image = await FileManage.UploadAsync(request.Image, _environment);

                    string id = await _userRepository.Signup(addUserRequest);
                    if (id!=null)
                    {
                        string token = GetTokenByRole(id, addUserRequest.RoleId);
                        response.Data = token;
                        response.Message = GetRegistrationMessage(addUserRequest.RoleId);
                        return Ok(response);
                    }
                    else
                    {
                        response.Message = MessageDisplay.error;
                        return BadRequest(response);
                    }
                }
                catch (Exception ex)
                {
                    response.Message = ex.Message.Contains("Duplicate entry") 
                        ? MessageDisplay.emailduplicated 
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

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] UserLoginRequest request)
        {
            ResponseDTO response = new();
            try
            {
                var user = await _userRepository.LoginEmailPass(request);

                if (user == null || !EncryptionDecryption.Match(request.Password, user.PasswordHash))
                {
                    return BadRequest(MessageDisplay.LoginIncorrectDetailMessage);
                }

                string token = GetTokenByRole(user.Id, user.RoleId);
                response.Message = MessageDisplay.LoginSuccessMessage;
                response.Data = token;
                return Ok(response);
            }
            catch (Exception)
            {
                response.Message = MessageDisplay.error;
                return BadRequest(response);
            }
        }

        private string GetTokenByRole(string id, int roleId)
        {
            return roleId switch
            {
                1 => _tokenHelper.GenerateToken(id.ToString(), "Admin"),
                2 => _tokenHelper.GenerateToken(id.ToString(), "Customer"),
                3 => _tokenHelper.GenerateToken(id.ToString(), "Manager"),
                4 => _tokenHelper.GenerateToken(id.ToString(), "DeliveryBoy"),
                _ => "Invalid role specified."
            };
        }

        private string GetRegistrationMessage(int roleId)
        {
            return roleId switch
            {
                1 => "Admin registered successfully.",
                2 => "Customer registered successfully.",
                3 => "Manager registered successfully.",
                4 => "Delivery Boy registered successfully.",
                _ => "User registered successfully."
            };
        }

        [HttpPost]
        public async Task<IActionResult> AddRole([FromBody] AddRoleRequest request)
        {
            ResponseDTO response = new();
            try
            {
                var role = Request.GetRole();
                if (role == "Admin")
                {
                    var addedRole = await _userRepository.AddRole(request);
                    response.Data = addedRole;
                    response.Message = "Role added successfully";
                    return Ok(response);
                }
                else
                {
                    response.Message = MessageDisplay.auth;
                    return Unauthorized(response);
                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message.Contains("Duplicate entry") 
                    ? MessageDisplay.Roleduplicated 
                    : MessageDisplay.error;
                return BadRequest(response);
            }
        }

        // Continue with other controller methods...
        // The pattern remains the same - remove Dapper dependencies and use the repository methods
    }
}
