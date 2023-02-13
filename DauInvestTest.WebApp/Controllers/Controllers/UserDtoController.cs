using AutoMapper;
using Common.Interfaces;
using DAL;
using DAL.Models;
using DataTransferObjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DauInvestTest.WebApp.Controllers.Controllers;

public class UserDtoController:BaseController<UserDto>
{
    public UserDtoController(ILogger<UserDto> logger, UserManager<User> userManager, IConfiguration configuration, IMapper mapper, AppDbContext context, IRepository<UserDto> userRepo) : base(logger, userManager, configuration, mapper, context,userRepo)
    {
    }
    
    [HttpGet("")]
    public async Task<IActionResult> GetProfile()
    {
        var user = await GetUserModelAsync();
        
        if (user == null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    [HttpPut("update")]
    public async Task<IActionResult> UpdateProfile([FromBody]UserDto delta, CancellationToken cancellationToken)
    {
        var user = await GetUserModelAsync();
        
        if (await _context.Users.AnyAsync(r => r.PhoneNumber == user.PhoneNumber && r.Id != user.Id, cancellationToken: cancellationToken))
        {
            {
                return BadRequest("Phone already exists");
            }
        }
        
        await _context.PatchAsync(delta, user.Id);
        
        return NoContent();
    }

    
}