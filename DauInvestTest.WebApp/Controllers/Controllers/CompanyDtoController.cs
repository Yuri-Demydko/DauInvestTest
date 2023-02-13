using AutoMapper;
using Common.Interfaces;
using DAL;
using DAL.Models;
using DataTransferObjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DauInvestTest.WebApp.Controllers.Controllers;

public class CompanyDtoController : BaseController<CompanyDto>
{
    private readonly IRepository<CompanyDto> _companyRepository;
    private readonly IRepository<UserDto> _userRepository;

    public CompanyDtoController(ILogger<CompanyDto> logger, UserManager<User> userManager, IConfiguration configuration, IMapper mapper, AppDbContext context, IRepository<CompanyDto> companyRepository, IRepository<UserDto> userRepository) : base(logger, userManager, configuration, mapper, context,userRepository)
    {
        _companyRepository = companyRepository;
        _userRepository = userRepository;
    }

    [HttpGet("")]
    public async Task<IActionResult> Get()
    {
        var user = await GetUserModelAsync();

        var item= await _companyRepository.Get()
            .FirstOrDefaultAsync(item => item.OwnerId == user.Id);
        
        return item != null ? Ok(item) : NotFound();
    }

    [HttpPost("")]
    public async Task<IActionResult> Post([FromBody]CompanyDto item)
    {
        var user = await GetUserModelAsync();

        if (user.CompanyId != null)
        {
            return BadRequest("Already have company");
        }
        
        var exist = await _companyRepository.Get()
            .AnyAsync(dto =>
                dto.Title == item.Title || dto.BusinessIdentificationNumber == item.BusinessIdentificationNumber);
        
        if (exist)
        {
            return BadRequest("Company with such title/BIN already exist");
        }

        item.OwnerId = user.Id;
        item = await _companyRepository.CreateAsync(item);
        
        user.CompanyId = item.Id;

        await _userRepository.UpdateAsync(user, true);
        
        return Ok(item);
    }

    [HttpPut("update")]
    public async Task<IActionResult> Update([FromBody] CompanyDto delta)
    {
        var user = await GetUserModelAsync();
    
        if (user.CompanyId == null)
        {
            return NotFound("There aren't your company");
        }
    
        var company = await _companyRepository.Get().FirstAsync(r => r.OwnerId == user.Id);
        delta.Id = company.Id;
        company=await _companyRepository.UpdateAsync(delta);
    
        return Ok(company);
    }

    [HttpPost("add-user")]
    public async Task<IActionResult> AddUser(string phone)
    {
        var user = await GetUserModelAsync();
        if (user.CompanyId == null)
        {
            return NotFound("There aren't your company");
        }

        var newMember = await _userRepository.Get()
            .FirstOrDefaultAsync(r => r.PhoneNumber == phone);
        if (newMember == null)
        {
            return NotFound("User with such phone doesn't exist");
        }
        var company = await _companyRepository.Get().FirstAsync(r => r.OwnerId == user.Id);
        
        newMember.CompanyId = company.Id;
        await _userRepository.UpdateAsync(newMember,true);

        return Ok();
    }
}