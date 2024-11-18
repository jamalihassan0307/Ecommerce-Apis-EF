using Ecommerce_Apis.UserModule.DTOs;
using Ecommerce_Apis.UserModule.Models;

namespace Ecommerce_Apis.UserModule.Repositories.InterFace
{
    public interface IUserRepository
    {
        Task<int> Signup(GetUserResponse request);
        Task<UserModel> LoginEmailPass(UserLoginRequest model);
        Task<List<GetAllUsersResponseWithRoleId>> GetAllUser();
        Task<bool> UpdateUser(GetUpdateRequest request);
        Task<bool> UpdateUserByAdmin(GetUpdateByAdminRequest request);
        Task<GetUserResponse> GetUserById(string id);
        Task<int> AddRole(AddRoleRequest request);
        Task<List<GetRole>> GetAllRoles();
        Task<GetRoleName> GetRolesByID(int id);
        Task<bool> UpdateRole(UpdateRoleRequest request);
        Task<bool> ResetPassword(ResetPasswordDTO request);
        Task<string> GetPassword(string userId);
        Task<bool> DeleteRole(int id);
        Task<bool> DeleteUserById(string userId);
    }
}

