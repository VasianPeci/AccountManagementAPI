using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using AccountManagement.Repositories;
using AccountManagement.Models.Identity;
using AccountManagement.DTO;

namespace AccountManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ITokenRepository tokenRepository;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            ITokenRepository tokenRepository)
        {
            this.userManager = userManager;
            this.tokenRepository = tokenRepository;
        }

        // Login User
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequestDto)
        {
            var user = await userManager.FindByEmailAsync(
                loginRequestDto.Username
            );

            if (user != null)
            {
                var checkPasswordResult = await userManager.CheckPasswordAsync(
                    user,
                    loginRequestDto.Password
                );

                if (checkPasswordResult)
                {
                    // Get Roles
                    var roles = await userManager.GetRolesAsync(user);

                    // Create Token
                    var jwtToken = tokenRepository.CreateJWTToken(
                        user,
                        roles.ToList()
                    );

                    var response = new LoginResponseDto
                    {
                        JwtToken = jwtToken
                    };

                    return Ok(response);
                }
            }

            return BadRequest("Username or password incorrect!");
        }
    }
}