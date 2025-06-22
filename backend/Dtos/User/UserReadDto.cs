namespace todoApp.Dtos.User;

using System.ComponentModel.DataAnnotations;

public class UserReadDto
{
    /// <summary>
    /// ID do usuário, gerado automaticamente pelo banco de dados.
    /// </summary>
    public int Id { get; set; }
    /// <summary>
    /// Email do usuário, deve ser único e válido.
    /// </summary>
    public required string Email { get; set; }
    /// <summary>
    /// Nome de usuário, deve ter entre 6 e 50 caracteres e pode conter letras, números e sublinhados.
    /// </summary>
    public required string Username { get; set; }

    /// <summary>
    /// Data de criação do usuário, gerada automaticamente pelo banco de dados.
    /// </summary>
    [DataType(DataType.DateTime)] // opcional — apenas para gerar melhor formatação em documentações Swagger
    [Display(Name = "Data de Criação")] // também opcional

    public DateTime CreatedAt { get; set; }

}
