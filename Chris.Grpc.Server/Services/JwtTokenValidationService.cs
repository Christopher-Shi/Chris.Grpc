using System;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace Chris.Grpc.Server.Services
{
    public class TokenModel
    {
        public string Token { get; set; }
        public DateTime Expiration { get; set; }
        public bool Success { get; set; }
    }

    public class UserModel
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }
    }

    public class JwtTokenValidationService
    {
        public async Task<TokenModel> GenerateTokenAsync(UserModel model)
        {
            if (model.UserName == "admin" && model.Password == "1234")
            {
                var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, "email@126.com"),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.UniqueName, "admin")
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("1-246542-123243-1422423-764784-0642-47692-401234"));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    "localhost",
                    "localhost",
                    claims,
                    expires: DateTime.Now.AddMinutes(10),
                    signingCredentials: creds
                    );

                return new TokenModel
                {
                    Token = new JwtSecurityTokenHandler().WriteToken(token),
                    Expiration = token.ValidTo,
                    Success = true
                };
            }

            return new TokenModel
            {
                Success = false
            };
        }
    }
}
