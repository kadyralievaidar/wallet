using System.ComponentModel.DataAnnotations;

namespace wallet.api.Features.Users.Dtos;

public class CreateUserDto
{
    /// <summary>
    ///     User email
    /// </summary>
    [DataType(DataType.EmailAddress)]
    [Required]
    public string Email { get; set; }

    /// <summary>
    ///     User's password (optional)
    /// </summary>
    public string Password { get; set; }
}
