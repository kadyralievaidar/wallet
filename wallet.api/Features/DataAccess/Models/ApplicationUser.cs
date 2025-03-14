using Microsoft.AspNetCore.Identity;

namespace wallet.api.Features.DataAccess.Models;

public class ApplicationUser : IdentityUser<Guid>
{
    public bool IsDeleted { get; set; }
}
