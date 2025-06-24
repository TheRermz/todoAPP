namespace todoApp.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using todoApp.Dtos.Auth;
using todoApp.Models;
using todoApp.Helpers;
using System.Security.Principal;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    private readonly IConfiguration _configuration;
    private readonly JwtService _jwtService;

    public AuthController(AppDbContext context, IMapper mapper, IConfiguration configuration, JwtService jwtService)
    {
        _context = context;
        _mapper = mapper;
        _configuration = configuration;
        _jwtService = jwtService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> LoginAuth([FromBody] UserLoginDto userLoginDto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userLoginDto.Email);

        if (user == null)
        {
            return Unauthorized("Usuário não encontrado.");
        }

        var pwd = BCrypt.Net.BCrypt.Verify(userLoginDto.Password, user.PasswordHash);
        if (!pwd)
        {
            return Unauthorized("Senha incorreta.");
        }

        var token = _jwtService.GenerateJwtToken(user);

        var res = new LoginResponseDto
        {
            JwtToken = token,
            Username = user.Username
        };

        return Ok(res);

    }
}
