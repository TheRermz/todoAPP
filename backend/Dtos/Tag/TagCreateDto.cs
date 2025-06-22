namespace todoApp.Dtos.Tag;

using System.ComponentModel.DataAnnotations;

public class TagCreateDto
{
    /// <summary>
    /// Título da tag, obrigatório e deve ter entre 3 e 50 caracteres.
    /// </summary>
    [Required(ErrorMessage = "Título é obrigatório.")]
    [StringLength(50, ErrorMessage = "O título deve ter entre 3 e 50 caracteres.", MinimumLength = 3)]
    [Display(Name = "Título")]
    [DataType(DataType.Text)]
    public required string Title { get; set; }
}
