using Microsoft.AspNetCore.Identity;
using wallet.api.Features.Users.Dtos;

namespace wallet.api.Features.Users;

public interface IIdentityUserService
{
    Task<(IdentityResult, Guid id)> CreateUser(CreateUserDto createUserDto);
}
