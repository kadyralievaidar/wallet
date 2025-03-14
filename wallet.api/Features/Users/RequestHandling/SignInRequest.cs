using MediatR;
using wallet.api.Features.Users.Dtos;

namespace wallet.api.Features.Users.RequestHandling;

public class SignInRequest : IRequest<string>
{
    public CreateUserDto Dto { get; set; }

    public SignInRequest(CreateUserDto dto)
    {
        Dto = dto;
    }
}
