using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Win32;
using System.Security.Claims;
using Talabat.APIs.Dtos;
using Talabat.APIs.Error;
using Talabat.APIs.Extentions;
using Talabat.Core.Entities.Identity;
using Talabat.Core.Services;

namespace Talabat.APIs.Controllers
{

    public class AccountController : ApiBaseController
    {
        private readonly UserManager<AppUser> userManager;
        private readonly SignInManager<AppUser> signInManager;
        private readonly ITokenService tokenService;
        private readonly IMapper mapper;

        public AccountController(UserManager<AppUser> userManager
            , SignInManager<AppUser> signInManager,
            ITokenService tokenService ,
            IMapper mapper)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.tokenService = tokenService;
            this.mapper = mapper;
        }

        [HttpPost("Login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto model)
        {
            var user = await userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return Unauthorized(new ApiErrorResponse(401));

            var result = await signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (result.Succeeded is false)
                return Unauthorized(new ApiErrorResponse(401));

            return Ok(new UserDto()
            {
                DisplayName = user.UserName,
                Email = user.Email,
                //Token = "This will be token"
                Token = await tokenService.CreateTokenAsync(user, userManager)
                //Token = await tokenService.CreateTokenAsync(user )
            });
        }

        [HttpPost("Register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto model)
        {
            if (CheckEmailExist(model.Email).Result.Value)
                return BadRequest(new ApiValidationErrorResponse()
                { Errors = new string[] { "This Email is Already Exist"} });

            var user = new AppUser()
            {
                DisplayName = model.DisplayName,
                Email = model.Email,
                UserName = model.Email.Split('@')[0],
                PhoneNumber = model.PhoneNumber,
            };

            var result = await userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
                return BadRequest(new ApiErrorResponse(400));

            return Ok(new UserDto()
            {
                DisplayName = user.UserName,
                Email = user.Email,
                //Token = "This will be token"
                Token = await tokenService.CreateTokenAsync(user, userManager)
                //Token = await tokenService.CreateTokenAsync(user)

            });
        }

        [HttpGet("CurrentUser")]
        [Authorize]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);

            var user = await userManager.FindByEmailAsync(email);

            return Ok(new UserDto
            {
                Email = user.Email,
                DisplayName = user.DisplayName,
                Token = await tokenService.CreateTokenAsync(user, userManager)
            });
        }
        /*              */
        [HttpGet("address")]
        [Authorize]
        public async Task<ActionResult<AddressDto>> GetUserAddress()
        {
            var user = await userManager.FindUserWithAddressByEmailAsync(User);

            var address = mapper.Map<Address, AddressDto>(user.Address);
           
            return Ok(address);
        }
        /*   */
        [Authorize]
        [HttpPut("address")]
        public async Task<ActionResult<AddressDto>> UpdateUserAddress(AddressDto UpdateAddress)
        {
            var address = mapper.Map<AddressDto, Address>(UpdateAddress);

            var user = await userManager.FindUserWithAddressByEmailAsync(User);

            user.Address = address; 

            var result = await userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return BadRequest(new ApiErrorResponse(400));

            return Ok(UpdateAddress);
        }

        [HttpGet("CheckEmail")]
        public async Task<ActionResult<bool>> CheckEmailExist(string email)
        {
            return await userManager.FindByEmailAsync(email) is not null;
        }
         



    }
}
