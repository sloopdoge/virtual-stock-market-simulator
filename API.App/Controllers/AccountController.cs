using API.Domain.Entities;
using API.Domain.Models;
using API.Identity.Entities;
using API.Identity.Interfaces;
using API.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.App.Controllers;

[ApiController]
[Route($"api/[controller]")]
public class AccountController(
    IUserService userService,
    IRoleService roleService,
    UserManager<ApplicationUser> userManager,
    RoleManager<ApplicationRole> roleManager,
    IJwtTokenService jwtTokenService) 
    : ControllerBase
{
    [AllowAnonymous]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthModel))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] UserLoginModel loginModel)
    {
        try
        {
            var returnModel = new AuthModel();

            if (string.IsNullOrEmpty(loginModel.Email) || string.IsNullOrEmpty(loginModel.Password))
            {
                returnModel.Succeeded = false;
                returnModel.Errors = [$"Incorrect input data"];
                    
                return BadRequest(returnModel);
            }

            var user = await userService.GetByEmail(loginModel.Email);
            if (user == null)
            {
                returnModel.Succeeded = false;
                returnModel.Errors = [$"User not found"];
                    
                return BadRequest(returnModel);
            }

            var isPasswordCorrect = await userManager.CheckPasswordAsync(user, loginModel.Password);
            if (!isPasswordCorrect)
            {
                returnModel.Succeeded = false;
                returnModel.Errors = [$"Wrong password"];
                    
                return BadRequest(returnModel);
            }

            var userRole = (await userManager.GetRolesAsync(user)).FirstOrDefault();
            if (userRole == null || string.IsNullOrEmpty(userRole))
            {
                returnModel.Succeeded = false;
                returnModel.Errors = [$"Incorrect user role"];
                    
                return BadRequest(returnModel);
            }

            var role = await roleManager.Roles.FirstOrDefaultAsync(r => string.Equals(r.NormalizedName, userRole.ToUpperInvariant()));

            if (role == null || string.IsNullOrEmpty(role.Name))
            {
                returnModel.Succeeded = false;
                returnModel.Errors = [$"User role not found"];
                    
                return BadRequest(returnModel);
            }

            var token = jwtTokenService.GenerateToken(user, role, loginModel.RememberMe);

            if (string.IsNullOrEmpty(token.Token))
            {
                returnModel.Succeeded = false;
                returnModel.Errors = [$"Token was not created"];
                    
                return BadRequest(returnModel);
            }

            returnModel.Succeeded = true;
            returnModel.UserId = user.Id;
            returnModel.RoleName = role.Name;
            returnModel.UserFullName = $"{user.FirstName} {user.LastName}";
            returnModel.UserEmail = user.Email ?? "";
            returnModel.Token = token;
            returnModel.Errors = [];

            return Ok(returnModel);
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }

    [AllowAnonymous]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status418ImATeapot)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpPost("Register")]
    public async Task<IActionResult> Register([FromBody] UserRegisterModel model)
    {
        try
        {
            var user = await userService.GetByEmail(model.Email);
            if (user == null)
                return BadRequest($"User with this email exists");

            var role = await roleService.GetById(model.Role.Id);

            if (role == null || string.IsNullOrEmpty(role.Name))
                return NotFound($"User role not found");

            var createdUser = await userService.Create(new AppUser(model));
            if (createdUser == null)
                return StatusCode(418, $"User was not created");
            
            var passwordAssigned = await userManager.AddPasswordAsync(createdUser, model.Password);
            
            return passwordAssigned.Succeeded
                ? Ok()
                : StatusCode(418);
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }
}