using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using todoApp.Dtos.Tag;
using todoApp.Models;
using Microsoft.AspNetCore.Authorization;

namespace todoApp.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TagController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public TagController(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TagReadDto>>> GetAllTags()
    {
        var tags = await _context.Tags.ToListAsync();
        var tagsDto = _mapper.Map<List<TagReadDto>>(tags);
        return Ok(tagsDto);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TagReadDto>> GetTagById(int id)
    {
        var tag = await _context.Tags.FindAsync(id);
        if (tag == null)
            return NotFound("Tag não encontrada.");

        var tagDto = _mapper.Map<TagReadDto>(tag);
        return Ok(tagDto);
    }

    [HttpPost]
    public async Task<ActionResult<TagReadDto>> CreateTag(TagCreateDto tagCreateDto)
    {
        var exists = await _context.Tags.AnyAsync(t => t.Name == tagCreateDto.Name);
        if (exists)
            return BadRequest("Já existe uma tag com esse nome.");

        var tag = new Tag { Name = tagCreateDto.Name };

        try
        {
            _context.Tags.Add(tag);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return BadRequest("Erro ao criar tag.");
        }

        var tagDto = _mapper.Map<TagReadDto>(tag);
        return CreatedAtAction(nameof(GetTagById), new { id = tag.Id }, tagDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTag(int id, TagCreateDto tagUpdateDto)
    {
        var tag = await _context.Tags.FindAsync(id);
        if (tag == null)
            return NotFound("Tag não encontrada.");

        tag.Name = tagUpdateDto.Name;

        try
        {
            await _context.SaveChangesAsync();
            return Ok("Tag atualizada com sucesso.");
        }
        catch (DbUpdateException)
        {
            return BadRequest("Erro ao atualizar tag.");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTag(int id)
    {
        var tag = await _context.Tags
            .Include(t => t.TaskTags)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (tag == null)
            return NotFound("Tag não encontrada.");

        if (tag.TaskTags.Any())
            return BadRequest("Tag associada a tarefas, não pode ser excluída.");

        try
        {
            _context.Tags.Remove(tag);
            await _context.SaveChangesAsync();
            return NoContent();
        }
        catch (DbUpdateException)
        {
            return BadRequest("Erro ao excluir tag.");
        }
    }
}
