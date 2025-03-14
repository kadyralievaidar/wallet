using System.Security.Claims;

namespace wallet.api.Features.Core;
public static class HttpContextAccessorExtension
{
    public static Guid GetUserId(this IHttpContextAccessor httpContextAccessor)
    {
        var userId = httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)!;
        return Guid.Parse(userId.Value.ToString());
    }
}
