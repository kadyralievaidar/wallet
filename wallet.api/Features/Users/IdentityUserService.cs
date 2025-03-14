using Duende.IdentityServer.Extensions;
using Microsoft.AspNetCore.Identity;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using wallet.api.Features.Core;
using wallet.api.Features.DataAccess.Models;
using wallet.api.Features.IdentityServer;
using wallet.api.Features.UserBalances;
using wallet.api.Features.Users.Dtos;

namespace wallet.api.Features.Users;

public class IdentityUserService : IIdentityUserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IUserBalanceService _userBalanceService;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public IdentityUserService(IServiceProvider serviceProvider)
    {
        _userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        _userBalanceService = serviceProvider.GetRequiredService<IUserBalanceService>();
        _httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
        _signInManager = serviceProvider.GetRequiredService<SignInManager<ApplicationUser>>();
        _httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
    }

    /// <inheritdoc />
    public async Task<CreateUserDto> CreateUser(CreateUserDto createUserDto)
    {
        ArgumentNullException.ThrowIfNull(createUserDto);


        var applicationUser = new ApplicationUser()
        {
            Id = Guid.NewGuid(),
            UserName = createUserDto.Email
        };
        IdentityResult result;
        if (!createUserDto.Password.IsNullOrEmpty())
            result = await _userManager.CreateAsync(applicationUser, createUserDto.Password);
        else
            result = await _userManager.CreateAsync(applicationUser);

        if (result.Succeeded)
        {
            await _userManager.AddClaimAsync(applicationUser, new Claim(Consts.Sub, applicationUser.Id.ToString(), ClaimValueTypes.String));
            await _userBalanceService.SetBalanceAsync(applicationUser.Id);
            return createUserDto;
        }
        throw new InvalidOperationException();
    }

    public async Task<string> SignIn(CreateUserDto signInDto, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByNameAsync(signInDto.Email);
        if (user == null || !await _userManager.CheckPasswordAsync(user, signInDto.Password))
        {
            return string.Empty;
        }
        var client = _httpClientFactory.CreateClient();

        var parameters = new Dictionary<string, string>
            {
                { "grant_type", "password" },
                { "client_id", "wallet" },
                { "username", signInDto.Email },
                { "password", signInDto.Password },
                { "scope", "openid profile wallet" },
                { "client_secret", "super-secret-key" }
            };
        var content = new FormUrlEncodedContent(parameters);

        var response = await client.PostAsync("http://localhost:7000/connect/token", content, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            return string.Empty;
        }

        var responseString = await response.Content.ReadAsStringAsync();
        var tokenResponse = JsonSerializer.Deserialize<AccessTokenResponse>(responseString)!;

        await _signInManager.SignInAsync(user, isPersistent: true);

        return tokenResponse.AccessToken;
    }

    public async Task SignOut()
    {
        using var client = _httpClientFactory.CreateClient();
        var request = new HttpRequestMessage(HttpMethod.Post, "https://localhost:7001/connect/revocation");
        var token = _httpContextAccessor.HttpContext.Request.Headers.Authorization;
        var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes("wallet:super-secret-key"));
        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", credentials);
        request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            { "token", token },
            { "token_type_hint", "access_token" }
        });

        var response = await client.SendAsync(request);
        var responseContent = await response.Content.ReadAsStringAsync();
        await _signInManager.SignOutAsync();

    }
}
