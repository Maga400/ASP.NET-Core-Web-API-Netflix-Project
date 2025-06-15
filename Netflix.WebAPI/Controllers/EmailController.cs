using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Netflix.Business.Services.Abstracts;
using Netflix.Entities.Models;
using Netflix.WebAPI.Dtos;
using Netflix.WebAPI.Services.Abstracts;

namespace Netflix.WebAPI.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly IEmailService _emailService;
        private readonly ICustomIdentityUserService _customIdentityUserService;
        private readonly UserManager<CustomIdentityUser> _userManager;
        public EmailController(IEmailService emailService, ICustomIdentityUserService customIdentityUserService, UserManager<CustomIdentityUser> userManager)
        {
            _emailService = emailService;
            _customIdentityUserService = customIdentityUserService;
            _userManager = userManager;
        }

        [HttpPost("emailVerificationCode")]
        public IActionResult SendVerificationCode([FromBody] EmailRequestDto dto)
        {
            try
            {
                var code = _emailService.SendVerificationCode(dto.Email);
                return Ok(new { Message = "Verification code sent successfully.", Code = code });
            }
            catch (Exception ex) 
            {
                return StatusCode(500, new { Message = "Error sending verification code.", Error = ex.Message });
            }
        }

        [HttpPost("verificationCode")]
        public IActionResult CheckVerificationCode([FromQuery] int code)
        {
            var result = _emailService.CheckVerificationCode(code);
            if (result.IsValid)
            {
                return Ok(new { Message = result.Message });
            }

            return BadRequest(new { Message = result.Message });
        }
    }
}
