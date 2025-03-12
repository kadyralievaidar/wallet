using System.ComponentModel.DataAnnotations;

namespace wallet.api.Features.Users.Dtos;

public class CreateUserDto
{
    /// <summary>
    ///     User id
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    ///     User email
    /// </summary>
    [DataType(DataType.EmailAddress)]
    [Required]
    public string Email { get; set; }

    /// <summary>
    ///     User's external userId
    /// </summary>
    public string ExternalUserId { get; set; }

    /// <summary>
    ///     User's external user name
    /// </summary>
    public string ExternalUserName { get; set; }

    /// <summary>
    ///     User's password (optional)
    /// </summary>
    public string Password { get; set; }

    /// <summary>
    ///     User's first name
    /// </summary>
    public string FirstName { get; set; }

    /// <summary>
    ///     User's last name
    /// </summary>
    public string LastName { get; set; }
}
