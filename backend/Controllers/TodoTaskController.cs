using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using todoApp.Dtos.TodoTask;
using todoApp.Models;
namespace todoApp.Controllers;

[Authorize]
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

    [HttpPost]
    public async Task<ActionResult<TodoTaskReadDto>> CreateTodoTask(TodoTaskCreateDto todoTaskCreateDto)
    {

        // Verificar se o usuário existe
        var userExists = await _context.Users.AnyAsync(u => u.Id == todoTaskCreateDto.UserId);
        if (!userExists)
        {
            return BadRequest("Usuário não encontrado para a tarefa.");
        }
        var taskExists = await _context.TodoTasks.AnyAsync(t => t.Title == todoTaskCreateDto.Title);
        if (taskExists)
        {
            return BadRequest("Tarefa com este título já existe.");
        }

        var task = _mapper.Map<TodoTask>(todoTaskCreateDto);

        task.CreatedAt = DateTime.UtcNow;
        task.UpdatedAt = DateTime.UtcNow;

        if (task.EndDate.HasValue && task.EndDate.Value.Kind == DateTimeKind.Unspecified)
            task.EndDate = DateTime.SpecifyKind(task.EndDate.Value, DateTimeKind.Utc);

        if (todoTaskCreateDto.TagIds != null && todoTaskCreateDto.TagIds.Any())
        {
            var tags = await _context.Tags
                .Where(tag => todoTaskCreateDto.TagIds.Contains(tag.Id))
                .ToListAsync();

            var notFoundIds = todoTaskCreateDto.TagIds.Except(tags.Select(t => t.Id)).ToList();
            if (notFoundIds.Any())
            {
                return BadRequest($"As tags com os seguintes IDs não foram encontradas: {string.Join(", ", notFoundIds)}");
            }

            task.TaskTags = tags.Select(tag => new TaskTag { TagId = tag.Id, Tag = tag }).ToList();
        }

        try
        {
            _context.TodoTasks.Add(task);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return BadRequest("Erro ao criar a tarefa.");
        }

        var todoTaskDto = _mapper.Map<TodoTaskReadDto>(task);
        return CreatedAtAction(nameof(GetTaskById), new { id = task.Id }, todoTaskDto);
    }


    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTodoTask(int id, TodoTaskUpdateDto dto)
    {
        var task = await _context.TodoTasks
            .Include(t => t.TaskTags)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (task == null)
        {
            return NotFound("Tarefa não encontrada.");
        }

        // Atualiza os campos se vierem preenchidos
        if (!string.IsNullOrWhiteSpace(dto.Title)) task.Title = dto.Title;
        if (!string.IsNullOrWhiteSpace(dto.Description)) task.Description = dto.Description;
        if (dto.StartDate.HasValue) task.StartDate = dto.StartDate.Value;
        if (dto.EndDate.HasValue) task.EndDate = dto.EndDate.Value;
        if (dto.Status.HasValue) task.Status = dto.Status.Value;
        if (dto.Priority.HasValue) task.Priority = dto.Priority.Value;

        task.UpdatedAt = DateTime.UtcNow;

        // Atualiza as tags se vierem novas
        if (dto.TagIds != null)
        {
            task.TaskTags.Clear();
            var tags = await _context.Tags.Where(t => dto.TagIds.Contains(t.Id)).ToListAsync();
            task.TaskTags = tags.Select(tag => new TaskTag { TagId = tag.Id, Tag = tag }).ToList();
        }

        try
        {
            await _context.SaveChangesAsync();
            return Ok("Tarefa atualizada com sucesso.");
        }
        catch (DbUpdateException)
        {
            return BadRequest("Erro ao atualizar a tarefa.");
        }
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTodoTask(int id)
    {
        var task = await _context.TodoTasks
    .Include(t => t.TaskTags)
    .FirstOrDefaultAsync(t => t.Id == id);

        if (task == null)
        {
            return NotFound("Tarefa não encontrada.");
        }

        _context.TodoTasks.Remove(task);

        try
        {
            await _context.SaveChangesAsync();
            return NoContent(); // status 204
        }
        catch (DbUpdateException)
        {
            return BadRequest("Erro ao excluir tarefa.");
        }
    }



}
