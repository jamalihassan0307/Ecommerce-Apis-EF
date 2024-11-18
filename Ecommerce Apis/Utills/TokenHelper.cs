using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Ecommerce_Apis.Utills
{
    public class TokenHelper
    {
        private IConfiguration _configuration;
        public TokenHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string GenerateToken(string UserId)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var Sectoken = new JwtSecurityToken(_configuration["Jwt:Issuer"],
                _configuration["Jwt:Issuer"],
                claims: [
                    new Claim("UserId", UserId)
                ],
                expires: DateTime.Now.AddDays(2),
                signingCredentials: credentials);

            var token = new JwtSecurityTokenHandler().WriteToken(Sectoken);

            return token;
        }
    }

}
