namespace todoApp.Dtos.TodoTask;

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using todoApp.Models;

public class TodoTaskUpdateDto
{
    /// <summary>
    /// Título da tarefa, obrigatório e deve ter entre 3 e 100 caracteres.
    /// </summary>
    [StringLength(100, ErrorMessage = "O título deve ter entre 3 e 100 caracteres.", MinimumLength = 3)]
    public string? Title { get; set; }

    /// <summary>
    /// Descrição da tarefa, opcional e deve ter no máximo 500 caracteres.
    /// </summary>
    [StringLength(500, ErrorMessage = "A descrição deve ter no máximo 500 caracteres.")]
    public string? Description { get; set; }

    /// <summary>
    /// Data de início da tarefa, obrigatória.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// Data de término da tarefa, opcional.
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Status da tarefa, opcional.
    /// </summary>
    [EnumDataType(typeof(Status), ErrorMessage = "Status inválido.")]
    public Status? Status { get; set; }

    /// <summary>
    /// Prioridade da tarefa, opcional.
    /// </summary>
    [EnumDataType(typeof(Priority), ErrorMessage = "Prioridade inválida.")]
    public Priority? Priority { get; set; }

    /// <summary>
    /// Lista de IDs de tags associadas à tarefa, opcional.
    /// </summary>
    [Display(Name = "Tags")]
    public List<int>? TagIds { get; set; }
}
