using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Netflix.Business.Services.Abstracts;
using Netflix.Entities.Models;
using Netflix.WebAPI.Dtos;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Netflix.WebAPI.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<CustomIdentityUser> _userManager;
        private readonly SignInManager<CustomIdentityUser> _signInManager;
        private readonly RoleManager<CustomIdentityRole> _roleManager;
        private readonly ICustomIdentityUserService _customIdentityUserService;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AuthController(UserManager<CustomIdentityUser> userManager, SignInManager<CustomIdentityUser> signInManager, RoleManager<CustomIdentityRole> roleManager, IConfiguration configuration, IMapper mapper, ICustomIdentityUserService customIdentityUserService, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _mapper = mapper;
            _customIdentityUserService = customIdentityUserService;
            _httpContextAccessor = httpContextAccessor;
        }
         
        [HttpPost("existUser")]
        public async Task<IActionResult> ExistUser([FromQuery] string email)
        {
            var existingUser = await _userManager.FindByEmailAsync(email);
            if (existingUser != null)
            {
                return BadRequest(new { Status = "Email Error", Message = "A user with this email already exists!" });
            }

            return Ok(new { Status = "Success" });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var user = new CustomIdentityUser
            {
                UserName = dto.Username,
                Email = dto.Email,
                ImagePath = dto.ImagePath,
                //CarType = dto.CarType,
                //PhoneNumber = dto.PhoneNumber,
                //Name = dto.Name,
                //Surname = dto.Surname,
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (result.Succeeded)
            {
                if (!await _roleManager.RoleExistsAsync(dto.Role))
                {
                    await _roleManager.CreateAsync(new CustomIdentityRole { Name = dto.Role });
                }

                await _userManager.AddToRoleAsync(user, dto.Role);

                return Ok(new { status = "Success", message = "User created successfuly!", error = "AUTH" });
            }

            return BadRequest(new { status = "Error", message = "User creation failed!", error = result.Errors });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null) return Unauthorized(new { message = "Invalid Credentials", error = "AUTH" });

            var result = await _signInManager.PasswordSignInAsync(user.UserName, dto.Password, false, false);

            if (result.Succeeded)
            {
                //var user = await _userManager.FindByEmailAsync(dto.Email);

                //if (user.IsBan)
                //{
                //    return BadRequest(new { Message = "This user has been banned by Admin", Error = "BAN" });
                //}

                var userRoles = await _userManager.GetRolesAsync(user);

                var authClaims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Email,user.Email),
                        new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                    };

                foreach (var role in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, role));
                }

                var token = GetToken(authClaims);

                var identity = new ClaimsIdentity(authClaims, "Registration");
                var principal = new ClaimsPrincipal(identity);

                _httpContextAccessor.HttpContext!.User = principal;

                return Ok(new { Token = new JwtSecurityTokenHandler().WriteToken(token), Expiration = token.ValidTo });

            }

            return Unauthorized(new { message = "Invalid Credentials", error = "AUTH" });
        }

        private JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            var authSignInKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Issuer"],
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSignInKey, SecurityAlgorithms.HmacSha256)
                );

            return token;
        }

        [Authorize]
        [HttpGet("currentUser")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var email = HttpContext.User.FindFirst(ClaimTypes.Email)?.Value;

            var currentUser = await _customIdentityUserService.GetByEmailAsync(email);

            if (currentUser == null)
            {
                return NotFound(new { message = "Current user not found", error = "Current User" });
            }

            var userRole = await _userManager.GetRolesAsync(currentUser);
            var currentUserDto = _mapper.Map<UserDto>(currentUser);
            return Ok(new { user = currentUserDto, role = userRole });

        }

        [Authorize]
        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            var email = HttpContext.User.FindFirst(ClaimTypes.Email)?.Value;

            var currentUser = await _customIdentityUserService.GetByEmailAsync(email);

            if (currentUser != null)
            {
                await _customIdentityUserService.UpdateAsync(currentUser);
                return Ok(new { message = "User logouted succesfully" });

            }

            return NotFound(new { message = "User logouting failed" });
        }
    }
}
