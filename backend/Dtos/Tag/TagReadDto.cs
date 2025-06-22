namespace todoApp.Dtos.Tag;

using System.ComponentModel.DataAnnotations;

public class TagReadDto
{
    /// <summary>
    /// ID da tag, obrigatório e gerado automaticamente pelo banco de dados.
    /// </summary>
    public int Id { get; set; }
    /// <summary>
    /// Título da tag, obrigatório e deve ter entre 3 e 50 caracteres.
    /// </summary>
    public required string Title { get; set; }
}
