using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.Identity;
using Talabat.Core.Services;


namespace Talabat.Service
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration configuration;

        public TokenService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
       public async Task<string> CreateTokenAsync(AppUser user, UserManager<AppUser> userManager)
        {
            //private claims [user defined ]
            var AuthClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.GivenName ,user.DisplayName),
                new Claim(ClaimTypes.Email ,user.Email),
            };

            var userRoles = await userManager.GetRolesAsync(user);
            foreach (var Role in userRoles)
                AuthClaims.Add(new Claim(ClaimTypes.Role, Role));

            //Security Key 
            var AuthKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Key"]));
       
            //Register Claims
            var Token = new JwtSecurityToken(
                issuer : configuration["JWT:ValidIssuer"],
                audience : configuration["JWT:ValidAudience"],
                //expires : DateTime.Now.AddDays(double.Parse(configuration["JWT:ValidAudience"])) ,
               // expires: DateTime.Now.AddMinutes(120),
                expires: DateTime.Now.AddDays(30),
                claims:AuthClaims,
               signingCredentials:new SigningCredentials(AuthKey,SecurityAlgorithms.HmacSha256Signature)
                //signingCredentials:new SigningCredentials(AuthKey, SecurityAlgorithms.HmacSha256)
                );

            return new JwtSecurityTokenHandler().WriteToken(Token);


        }
    }
}
