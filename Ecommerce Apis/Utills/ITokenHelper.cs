namespace Ecommerce_Apis.Utills
{
    public interface ITokenHelper
    {
        string GenerateToken(string userId, string role);
    }
}
