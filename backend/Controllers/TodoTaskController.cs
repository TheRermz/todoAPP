using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using todoApp.Dtos.TodoTask;
using todoApp.Models;
namespace todoApp.Controllers;

[ApiController]
[Route("api/[controller]")]

public class TodoTaskController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public TodoTaskController(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<TodoTaskReadDto>> GetAllTasks()
    {
        var tasks = await _context.TodoTasks.ToListAsync();
        var tasksDto = _mapper.Map<List<TodoTaskReadDto>>(tasks);
        return Ok(tasksDto);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TodoTaskReadDto>> GetTaskById(int id)
    {
        var task = await _context.TodoTasks.FindAsync(id);
        if (task == null)
        {
            return NotFound("Tarefa não encontrada");
        }
        var taskDto = _mapper.Map<TodoTaskReadDto>(task);
        return Ok(taskDto);
    }
}
