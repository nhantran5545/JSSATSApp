using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace JSSATSAPI.BussinessObjects.InheritanceClass
{
    public class ProvideToken
    {
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _memoryCache;
        private static ProvideToken _instance;

        public static ProvideToken Instance => _instance;

        private ProvideToken(IConfiguration configuration, IMemoryCache memoryCache)
        {
            _configuration = configuration;
            _memoryCache = memoryCache;
        }

        public static void Initialize(IConfiguration configuration, IMemoryCache memoryCache)
        {
            if (_instance == null)
                _instance = new ProvideToken(configuration, memoryCache);
        }

        public (string token, string role) GenerateToken(int accountId, string role)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var secretKey = _configuration["AppSettings:SecretKey"];
            var key = Encoding.ASCII.GetBytes(secretKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim("AccountId", accountId.ToString()),
                    new Claim(ClaimTypes.Role, role),
                }),
                Expires = DateTime.UtcNow.AddHours(6),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            _memoryCache.Set(accountId.ToString(), tokenString, TimeSpan.FromMinutes(10));

            return (tokenString, role);
        }
    }
}
