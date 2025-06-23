namespace todoApp.Dtos.TodoTask;

using System.ComponentModel.DataAnnotations;
using todoApp.Models;

public class TodoTaskCreateDto
{
    /// <summary>
    /// Título da tarefa, obrigatório e deve ter entre 3 e 100 caracteres.
    /// </summary>
    [Required(ErrorMessage = "Título é obrigatório.")]
    [StringLength(100, ErrorMessage = "O título deve ter entre 3 e 100 caracteres.", MinimumLength = 3)]
    [Display(Name = "Título")]
    [DataType(DataType.Text)]
    public required string Title { get; set; }

    /// <summary>
    /// Descrição da tarefa, opcional.
    /// </summary>
    [StringLength(500, ErrorMessage = "A descrição deve ter no máximo 500 caracteres.")]
    [Display(Name = "Descrição")]
    [DataType(DataType.MultilineText)]
    public string? Description { get; set; }

    /// <summary>
    /// Data de início da tarefa, obrigatória.
    /// </summary>
    [Required(ErrorMessage = "Data de início é obrigatória.")]
    [Display(Name = "Data de Início")]
    [DataType(DataType.Date)]
    public required DateTime StartDate { get; set; }

    /// <summary>
    /// Data de término da tarefa, opcional.
    /// </summary>
    [Display(Name = "Data de Término")]
    [DataType(DataType.Date)]
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Status da tarefa, padrão é "Pendente".
    /// </summary>
    [Display(Name = "Status")]
    [EnumDataType(typeof(Status), ErrorMessage = "Status inválido.")]
    public Status Status { get; set; } = Status.Pending;

    /// <summary>
    /// Prioridade da tarefa, padrão é "Baixa".
    /// </summary>
    [Display(Name = "Prioridade")]
    [EnumDataType(typeof(Priority), ErrorMessage = "Prioridade inválida.")]
    public Priority Priority { get; set; } = Priority.Low;

    /// <summary>
    /// Lista de IDs de tags associadas à tarefa, opcional.
    /// </summary>
    [Display(Name = "Tags")]
    [DataType(DataType.Text)]
    public List<int>? TagIds { get; set; } = new();

    /// <summary>
    /// Data de criação da tarefa, preenchida automaticamente.
    /// </summary>
    [Display(Name = "Criado em")]
    [DataType(DataType.DateTime)]
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Associa a tarefa ao usuario
    /// </summary>
    public int UserId { get; set; }
}
