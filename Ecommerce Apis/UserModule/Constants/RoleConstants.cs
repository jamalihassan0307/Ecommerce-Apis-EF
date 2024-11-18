using Ecommerce_Apis.UserModule.Enums;

namespace Ecommerce_Apis.UserModule.Constants
{
    public static class RoleConstants
    {
        public static readonly Dictionary<int, string> RoleNames = new()
        {
            { (int)UserRoles.Admin, "Admin" },
            { (int)UserRoles.Customer, "Customer" },
            { (int)UserRoles.Manager, "Manager" },
            { (int)UserRoles.DeliveryBoy, "DeliveryBoy" }
        };

        public static string GetRoleName(int roleId)
        {
            return RoleNames.TryGetValue(roleId, out var roleName) ? roleName : "Unknown";
        }
    }
} 