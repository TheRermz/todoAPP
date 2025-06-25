using System.Security.Claims;
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

public class TodoTaskController : BaseController
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public TodoTaskController(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TodoTaskReadDto>>> GetAllTasks()
    {
        var userId = GetUserIdFromClaims();
        if (userId == null) return Unauthorized();

        var tasks = await _context.TodoTasks
            .Include(t => t.TaskTags)
            .ThenInclude(tt => tt.Tag)
            .Where(t => t.UserId == userId)
            .ToListAsync();

        var tasksDto = _mapper.Map<List<TodoTaskReadDto>>(tasks);
        return Ok(tasksDto);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TodoTaskReadDto>> GetTaskById(int id)
    {

        var userId = GetUserIdFromClaims();
        if (userId == null) return Unauthorized();

        var task = await _context.TodoTasks
            .Include(t => t.TaskTags)
            .ThenInclude(tt => tt.Tag)
            .Where(t => t.Id == id && t.UserId == userId)
            .FirstOrDefaultAsync(t => t.Id == id);

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

        var userId = GetUserIdFromClaims();
        if (userId == null) return Unauthorized();

        var taskExists = await _context.TodoTasks.AnyAsync(t => t.Title == todoTaskCreateDto.Title && t.UserId == userId);
        if (taskExists)
        {
            return BadRequest("Tarefa com este título já existe.");
        }

        var task = _mapper.Map<TodoTask>(todoTaskCreateDto);

        task.UserId = userId.Value;
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

        var taskWithTags = await _context.TodoTasks
            .Include(t => t.TaskTags)
            .ThenInclude(tt => tt.Tag)
            .FirstOrDefaultAsync(t => t.Id == task.Id);

        var todoTaskDto = _mapper.Map<TodoTaskReadDto>(taskWithTags);

        return CreatedAtAction(nameof(GetTaskById), new { id = task.Id }, todoTaskDto);
    }


    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTodoTask(int id, TodoTaskUpdateDto dto)
    {
        var userId = GetUserIdFromClaims();
        if (userId == null) return Unauthorized();

        var task = await _context.TodoTasks
            .Include(t => t.TaskTags)
            .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

        if (task == null) return NotFound("Tarefa não encontrada ou acesso negado.");

        // Atualiza os campos se vierem preenchidos
        if (!string.IsNullOrWhiteSpace(dto.Title))
        {
            var titleExists = await _context.TodoTasks
                .AnyAsync(t => t.Title == dto.Title && t.UserId == userId && t.Id != id);

            if (titleExists)
                return BadRequest("Outra tarefa com este título já existe.");
        }
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
            return Ok(dto);
        }
        catch (DbUpdateException)
        {
            return BadRequest("Erro ao atualizar a tarefa.");
        }
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTodoTask(int id)
    {
        var userId = GetUserIdFromClaims();
        if (userId == null) return Unauthorized();

        var task = await _context.TodoTasks
            .Include(t => t.TaskTags)
            .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

        if (task == null) return NotFound("Tarefa não encontrada ou acesso negado.");

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
