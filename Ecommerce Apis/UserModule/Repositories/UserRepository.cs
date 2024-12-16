using Microsoft.EntityFrameworkCore;
using Ecommerce_Apis.Data;
using Ecommerce_Apis.UserModule.DTOs;
using Ecommerce_Apis.UserModule.Models;
using Ecommerce_Apis.UserModule.Repositories.InterFace;
using Ecommerce_Apis.Utills;
using Ecommerce_Apis.UserModule.Enums;
using Ecommerce_Apis.UserModule.Constants;

namespace Ecommerce_Apis.UserModule.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ITokenHelper _tokenHelper;
        private readonly IConfiguration _configuration;

        public UserRepository(ApplicationDbContext context, ITokenHelper tokenHelper, IConfiguration configuration)
        {
            _context = context;
            _tokenHelper = tokenHelper;
            _configuration = configuration;
        }

        public async Task<string> Signup(GetUserResponse request)
        {
            // Set a default image path if none is provided
            var defaultImagePath = "/uploads/04800640-96cc-4e6f-bb7f-a8f31c4a5771.jpg";
            var user = new User
            {
                Id = Guid.NewGuid().ToString(),
                FullName = request.FullName,
                Email = request.Email,
                PasswordHash = request.PasswordHash,
                PhoneNumber = request.PhoneNumber,
                Image = string.IsNullOrEmpty(request.Image) ? defaultImagePath : request.Image,
                RoleId = request.RoleId
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user.Id;
        }


        public async Task<UserModel> LoginEmailPass(UserLoginRequest model)
        {
            return await _context.Users
                .Where(u => u.Email == model.Email)
                .Select(u => new UserModel
                {
                    Id = u.Id,
                    Email = u.Email,
                    FullName = u.FullName,
                    PasswordHash = u.PasswordHash,
                    PhoneNumber = u.PhoneNumber,
                    RoleId = u.RoleId,
                    Image = u.Image
                })
                .FirstOrDefaultAsync();
        }

        public async Task<List<GetAllUsersResponseWithRoleId>> GetAllUser()
        {
            return await _context.Users
                .Select(u => new GetAllUsersResponseWithRoleId
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber,
                    PasswordHash = u.PasswordHash,
                    Image = u.Image,
                    RoleId = u.RoleId,
                    RoleName = RoleConstants.GetRoleName(u.RoleId)
                })
                .ToListAsync();
        }

        public async Task<bool> UpdateUser(GetUpdateRequest request)
        {
            var user = await _context.Users.FindAsync(request.Id);
            if (user == null) return false;

            user.FullName = request.FullName;
            user.PhoneNumber = request.PhoneNumber;
            user.PasswordHash = request.Passwordhash;
            user.Image = request.Image;

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateUserByAdmin(GetUpdateByAdminRequest request)
        {
            var user = await _context.Users.FindAsync(request.Id);
            if (user == null) return false;

            user.FullName = request.FullName;
            user.PhoneNumber = request.PhoneNumber;
            user.Image = request.Image;
            user.RoleId = request.RoleId;

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<GetUserResponse> GetUserById(string id)
        {
            //var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

            //if (user != null)
            //{
            //    user.RoleId = 1; // Update the RoleId to the desired value
            //    await _context.SaveChangesAsync(); // Save changes to the database
            //}
            //else
            //{
            //    // Handle the case where the user with the specified ID is not found
            //    throw new Exception("User not found");
            //}

return await _context.Users
                .Where(u => u.Id == id)
                .Select(u => new GetUserResponse
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber,
                    Image = u.Image,
                    RoleId = u.RoleId
                })
                .FirstOrDefaultAsync();
        }

        public async Task<List<GetRole>> GetAllRoles()
        {
            return RoleConstants.RoleNames.Select(r => new GetRole
            {
                RoleId = r.Key,
                Name = r.Value
            }).ToList();
        }

        public async Task<GetRoleName> GetRolesByID(int id)
        {
            var roleName = RoleConstants.GetRoleName(id);
            return new GetRoleName { Name = roleName };
        }

        public async Task<bool> ResetPassword(ResetPasswordDTO request)
        {
            var user = await _context.Users.FindAsync(request.Id);
            if (user == null) return false;

            user.PasswordHash = request.Passwordhash;
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<string> GetPassword(string userId)
        {
            var user = await _context.Users.FindAsync(userId);
            return user?.PasswordHash;
        }

        public async Task<bool> DeleteUserById(string userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return false;

            _context.Users.Remove(user);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<int> AddRole(AddRoleRequest request)
        {
            throw new NotSupportedException("Roles are predefined and cannot be added dynamically.");
        }

        public async Task<bool> UpdateRole(UpdateRoleRequest request)
        {
            throw new NotSupportedException("Roles are predefined and cannot be modified.");
        }

        public async Task<bool> DeleteRole(int id)
        {
            throw new NotSupportedException("Roles are predefined and cannot be deleted.");
        }
    }
}

