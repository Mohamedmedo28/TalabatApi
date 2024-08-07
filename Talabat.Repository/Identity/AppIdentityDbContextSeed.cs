using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.Identity;

namespace Talabat.Repository.Identity
{
    public static class AppIdentityDbContextSeed
    {
         public static async Task SeedUsersAsync(UserManager<AppUser> UserManager)
        {
            if (UserManager.Users.Count() == 0)
            {
                var User = new AppUser()
                {
                    DisplayName = "Mohamed Ali",
                    Email = "Mohamed.Ali@Email.Com",
                    UserName = "Mohamed.Ali",
                    PhoneNumber = "01125550317"
                };
                await UserManager.CreateAsync(User ,"P@ss0rd");
            }
        }
    }
}
