using wallet.api.Features.Users.Dtos;

namespace wallet.api.Features.Users;

public interface IIdentityUserService
{
    Task<CreateUserDto> CreateUser(CreateUserDto createUserDto);

    Task<string> SignIn(CreateUserDto signInDto, CancellationToken cancellationToken);
    Task SignOut();
}
