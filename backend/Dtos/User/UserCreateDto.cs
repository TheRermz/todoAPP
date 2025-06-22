using System.ComponentModel.DataAnnotations;

namespace todoApp.Dtos.User;

public class UserCreateDto
{

    /// <summary>
    /// Email do usuário, deve ser único e válido.
    /// </summary>
    [Required(ErrorMessage = "Email é obrigatório.")]
    [EmailAddress(ErrorMessage = "Email inválido.")]
    public required string Email { get; set; }
    /// <summary>
    /// Nome de usuário, deve ter entre 6 e 50 caracteres e pode conter letras, números e sublinhados.
    /// </summary>
    [Required(ErrorMessage = "Nome de usuário é obrigatório.")]
    [StringLength(50, ErrorMessage = "O nome de usuário deve ter no máximo 50 caracteres.", MinimumLength = 6)]
    [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "O nome de usuário só pode conter letras, números e sublinhados.")]
    public required string Username { get; set; }
    /// <summary>
    /// Senha do usuário, deve ter entre 6 e 32 caracteres.
    /// </summary>
    [Required(ErrorMessage = "Senha é obrigatória.")]
    [StringLength(32, ErrorMessage = "A senha deve ter pelo menos 6 caracteres.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    public required string Password { get; set; }
}
