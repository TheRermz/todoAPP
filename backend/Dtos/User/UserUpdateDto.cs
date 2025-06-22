namespace todoApp.Dtos.User;

using System.ComponentModel.DataAnnotations;

public class UserUpdateDto
{
    /// <summary>
    /// Senha do usuário, deve ter entre 6 e 32 caracteres.
    /// </summary>
    [Required(ErrorMessage = "Senha é obrigatória.")]
    [StringLength(32, ErrorMessage = "A senha deve ter pelo menos 6 caracteres.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    public required string Password { get; set; }
}
