using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Talabat.Core.Entities.Identity;

namespace Talabat.APIs.Extentions
{
    public static class UserManagerExtensions
    {
        public static async Task<AppUser> FindUserWithAddressByEmailAsync(this UserManager<AppUser> userManager, ClaimsPrincipal currentUser)
        {
            var email = currentUser.FindFirstValue(ClaimTypes.Email);

            var user = await userManager.Users.Include(x => x.Address).SingleOrDefaultAsync(e=>e.Email == email);

            return user  ; 
        }
    }
}
