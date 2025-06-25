using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using todoApp.Dtos.User;
using todoApp.Models;
namespace todoApp.Controllers;

[ApiController]
[Route("api/[controller]")]

public class UserController : ControllerBase
{

    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public UserController(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    [Authorize]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserReadDto>>> GetAllUsers()
    {
        var users = await _context.Users.ToListAsync();
        var usersDto = _mapper.Map<List<UserReadDto>>(users);
        return Ok(usersDto);
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<ActionResult<UserReadDto>> GetUserById(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            return NotFound("Usuário não encontrado");
        }
        // Apenas retorna o próprio usuário
        var userIdFromToken = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        if (id != userIdFromToken)
        {
            return Forbid("Você não tem permissão para acessar este recurso.");
        }
        var userDto = _mapper.Map<UserReadDto>(user);
        return Ok(userDto);
    }

    [HttpPost]
    public async Task<ActionResult<UserReadDto>> CreateUser(UserCreateDto userCreateDto)
    {
        var user = _mapper.Map<User>(userCreateDto);
        var userExists = await _context.Users.AnyAsync(u => u.Username == user.Username);
        var emailExists = await _context.Users.AnyAsync(e => e.Email == user.Email);


        if (userExists)
        {
            return BadRequest("Usuário já cadastrado.");
        }
        else if (emailExists)
        {
            return BadRequest("Email já cadastrado.");
        }

        var pwdPlain = userCreateDto.Password;
        var pwdHash = BCrypt.Net.BCrypt.HashPassword(pwdPlain);
        user.PasswordHash = pwdHash;

        try
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return BadRequest("Erro ao criar usuário.");
        }

        var userDto = _mapper.Map<UserReadDto>(user);
        return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, userDto);
    }
    [Authorize]
    [HttpPatch("{id}")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateUser(int id, UserUpdateDto userUpdateDto)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            return NotFound("Usuário não encontrado.");
        }

        if (!string.IsNullOrWhiteSpace(userUpdateDto.Password) &&
            BCrypt.Net.BCrypt.Verify(userUpdateDto.Password, user.PasswordHash))
        {
            return BadRequest("A nova senha não pode ser igual à senha atual.");
        }

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(userUpdateDto.Password);
        user.UpdatedAt = DateTime.UtcNow;

        try
        {
            await _context.SaveChangesAsync();
            return Ok("Usuário atualizado com sucesso.");
        }
        catch (DbUpdateException)
        {
            return BadRequest("Erro ao atualizar usuário.");
        }
    }
    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            return NotFound("Usuário não encontrado.");
        }

        if (await _context.TodoTasks.AnyAsync(u => u.UserId == id))
        {
            return Conflict("Usuário possui tarefas. Remova ou transfira-as antes de excluir o usuário.");
        }
        try
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return NoContent();
        }
        catch (DbUpdateException)
        {
            return BadRequest("Erro ao excluir usuário.");
        }
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<UserReadDto>> GetCurrentUser()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null) return Unauthorized();

        if (!int.TryParse(userIdClaim.Value, out var userId)) return Unauthorized(); // é uma forma segura de garantir que você tenha um ID de usuário numérico válido antes de continuar. Se não for válido, a requisição é negada.

        var user = await _context.Users.FindAsync(userId);
        if (user == null) return NotFound("Usuário não encontrado");

        var userDto = _mapper.Map<UserReadDto>(user);
        return Ok(userDto);
    }


}
