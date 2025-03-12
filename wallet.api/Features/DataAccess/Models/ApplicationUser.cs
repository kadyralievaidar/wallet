using Microsoft.AspNetCore.Identity;

namespace wallet.api.Features.DataAccess.Models;

public class ApplicationUser : IdentityUser<Guid>
{
    public Guid Id { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastModifiedAt { get; set; } 
}
