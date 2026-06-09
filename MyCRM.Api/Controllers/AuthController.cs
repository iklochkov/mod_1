using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyCRM.Application.DTOs;
using MyCRM.Api.Services;

namespace MyCRM.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly IConfiguration _configuration;
    private readonly IJwtService _jwtService;
    
    public AuthController(
        UserManager<IdentityUser> userManager,
        IConfiguration configuration,
        IJwtService jwtService)
    {
        _userManager = userManager;
        _configuration = configuration;
        _jwtService = jwtService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(ClientRegisterDto dto)
    {
            var user = new IdentityUser
            {
            UserName = dto.Email,
            Email = dto.Email
            };

        var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
            {
                return BadRequest(new { error = result.Errors.Select(e => e.Description)});
            }

            await _userManager.AddToRoleAsync(user, "Manager");

            return Ok( new
            {
                message = "Пользователь создан", 
                userId = user.Id,
                role = "Manager"
            });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(ClientLoginDto dto)
        {

            var user = await _userManager.FindByEmailAsync(dto.Email);

            if( user == null)
            {
                return Unauthorized(new {error = "пользователя с таким email не найдено"});
            }

            var IsValidPassword = await _userManager.CheckPasswordAsync(user, dto.Password );

            if (!IsValidPassword)
            {
                return Unauthorized(new {error = "неверный пароль"});
            }

            var userRoles = await _userManager.GetRolesAsync(user);

            var token = _jwtService.GenerateToken(user, userRoles);

            return Ok (new
            {
            accessToken = token,
            email = user.Email,
            roles = userRoles.ToList()
            });
        }
    

    [HttpGet("me")]
    [Authorize] 
    public async Task<IActionResult> GetMe()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var user = await _userManager.FindByIdAsync(userId);

            if(user == null)
            {
                return NotFound(new {error = "пользователь не найден"});
            }

            var userRole = await _userManager.GetRolesAsync(user);

            return Ok( new
            {
            id = user.Id,
            email = user.Email,
            userName = user.UserName,
            emailConfirmed = user.EmailConfirmed,
            phoneNumber = user.PhoneNumber,
            roles = userRole.ToList(),
            });
        }

    [HttpGet("refresh")]
    [Authorize] 
    public async Task<IActionResult> Refresh()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var user = await _userManager.FindByIdAsync(userId);

            if(user == null)
            {
                return NotFound(new {error = "пользователь не найден"});
            }

            var userRoles = await _userManager.GetRolesAsync(user);

            var token = _jwtService.GenerateToken(user, userRoles);

            return Ok (new
            {
            accessToken = token
            });
        }

}
}

