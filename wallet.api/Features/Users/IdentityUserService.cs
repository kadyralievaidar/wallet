using AutoMapper;
using Duende.IdentityServer.Extensions;
using Microsoft.AspNetCore.Identity;
using wallet.api.Features.DataAccess.Models;
using wallet.api.Features.Payment;
using wallet.api.Features.UserBalances;
using wallet.api.Features.Users.Dtos;

namespace wallet.api.Features.Users;

public class IdentityUserService : IIdentityUserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUserBalanceService _userBalanceService;
    private readonly IMapper _mapper;

    public IdentityUserService(UserManager<ApplicationUser> userManager, IUserBalanceService userBalanceService, IMapper mapper)
    {
        _userManager = userManager;
        _userBalanceService = userBalanceService;
        _mapper = mapper;
    }

    /// <inheritdoc />
    public async Task<(IdentityResult, Guid id)> CreateUser(CreateUserDto createUserDto)
    {
        if (createUserDto == null)
            throw new ArgumentNullException(nameof(createUserDto));


        var applicationUser = _mapper.Map<ApplicationUser>(createUserDto);
        IdentityResult result;
        if (!createUserDto.Password.IsNullOrEmpty())
            result = await _userManager.CreateAsync(applicationUser, createUserDto.Password);
        else
            result = await _userManager.CreateAsync(applicationUser);

        await _userBalanceService.SetBalanceAsync(applicationUser.Id);
        return (result, applicationUser.Id);
    }
}
