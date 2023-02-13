using AutoMapper;
using Common.Interfaces;
using DAL;
using DAL.Models;
using DataTransferObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DauInvestTest.WebApp.Controllers.Controllers;

[Authorize]
[Route("api/[controller]")]
public abstract class BaseController<TModel>:ControllerBase
{
    protected readonly ILogger<TModel> _logger;
    protected readonly UserManager<User> _userManager;
    protected readonly IConfiguration _configuration;
    protected readonly IMapper _mapper;
    protected readonly AppDbContext _context;
    protected readonly IRepository<UserDto> _userRepository;

    protected BaseController(ILogger<TModel> logger, UserManager<User> userManager, IConfiguration configuration, IMapper mapper, AppDbContext context, IRepository<UserDto> userRepository)
    {
        _logger = logger;
        _userManager = userManager;
        _configuration = configuration;
        _mapper = mapper;
        _context = context;
        _userRepository = userRepository;
    }

    protected async Task<UserDto> GetUserModelAsync()
    {
        return User.Identity?.IsAuthenticated==false ? null : _mapper.Map<UserDto>(await _userRepository.Get().FirstOrDefaultAsync(r=>r.PhoneNumber==User.Identity.Name));
    }
    
}