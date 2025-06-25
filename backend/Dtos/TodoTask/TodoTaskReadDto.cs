namespace todoApp.Dtos.TodoTask;

using System.ComponentModel.DataAnnotations;
using todoApp.Dtos.Tag;
using todoApp.Models;

public class TodoTaskReadDto
{

    /// <summary>
    /// ID da tarefa, obrigatório e gerado automaticamente pelo banco de dados.
    /// </summary>
    public int Id { get; set; }
    /// <summary>
    /// Título da tarefa, obrigatório e deve ter entre 3 e 100 caracteres.
    /// </summary>
    public required string Title { get; set; }
    /// <summary>
    /// Descrição da tarefa, opcional e deve ter no máximo 500 caracteres.
    /// </summary>
    public string? Description { get; set; }
    /// <summary>
    /// Data de início da tarefa, obrigatória.
    /// </summary>
    public required DateTime StartDate { get; set; }
    /// <summary>
    /// Data de término da tarefa, opcional.
    /// Se não for fornecida, a tarefa é considerada sem data de término.
    /// </summary>
    public DateTime? EndDate { get; set; }
    /// <summary>
    /// Status da tarefa, obrigatório e deve ser um valor do enum Status.
    /// Os valores possíveis são: Pending, InProgress, Completed, Cancelled.
    /// O padrão é "Pending".
    /// </summary>
    public Status Status { get; set; }
    /// <summary>
    /// Prioridade da tarefa, obrigatória e deve ser um valor do enum Priority.
    /// Os valores possíveis são: Low, Medium, High.
    /// O padrão é "Low".
    /// Prioridade indica a importância da tarefa e pode influenciar na ordem de execução.
    /// </summary>
    public Priority Priority { get; set; }
    /// <summary>
    /// Lista de tags associadas à tarefa, opcional.
    /// Cada tag é representada por um objeto TagReadDto, que contém informações sobre a tag.
    /// As tags ajudam a categorizar e organizar as tarefas, facilitando a busca e
    /// filtragem por temas ou características específicas.
    /// </summary>
    public List<TagReadDto>? TagList { get; set; } = new();
    /// <summary>
    /// Data de criação da tarefa, obrigatória e gerada automaticamente pelo banco de dados.
    /// </summary>
    [DataType(DataType.DateTime)]
    [Display(Name = "Data de Criação")]
    public DateTime CreatedAt { get; set; }
    /// <summary>
    /// Data de atualização da tarefa, opcional e gerada automaticamente pelo banco de dados.
    /// Se a tarefa não foi atualizada, este campo pode ser nulo.
    /// </summary>
    [DataType(DataType.DateTime)]
    [Display(Name = "Data de Atualização")]
    public DateTime? UpdatedAt { get; set; }
}
