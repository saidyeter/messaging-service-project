using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace MessagingService.Api.Services
{
    public interface IJwtService
    {
        string CreateToken(Dictionary<string, string> claims);
        Dictionary<string, string> Resolve(string token);
    }

    public class JwtService : IJwtService
    {
        private readonly string JwtSecret;
        private readonly int TokenExpireIn;
        public JwtService(IConfiguration configuration)
        {
            JwtSecret = configuration.GetValue<string>("Security:JwtSecret");
            TokenExpireIn = configuration.GetValue<int>("Security:TokenExpireIn");
        }
        public string CreateToken(Dictionary<string, string> claims)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(JwtSecret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims.Select(x => new Claim(x.Key, x.Value))),
                Expires = DateTime.UtcNow.AddMinutes(TokenExpireIn),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        public Dictionary<string, string> Resolve(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return null;
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(JwtSecret);

            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            
            Dictionary<string, string> claims = new Dictionary<string, string>();

            jwtToken.Claims.ToList().ForEach(x => claims.Add(x.Type, x.Value));

            return claims;
        }
    }
}
