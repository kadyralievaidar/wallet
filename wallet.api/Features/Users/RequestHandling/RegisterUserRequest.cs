using MediatR;
using wallet.api.Features.Users.Dtos;

namespace wallet.api.Features.Users.RequestHandling;

public class RegisterUserRequest : IRequest<CreateUserDto>
{
    public CreateUserDto Dto { get; set; }

    public RegisterUserRequest(CreateUserDto dto)
    {
        Dto = dto;
    }
}
