namespace wallet.api.Features.Core;
public static class HttpContextAccessorExtension
{
    public static Guid GetUserId(this IHttpContextAccessor httpContextAccessor)
    {
        var userId = httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(x => x.Type == Consts.UserId)!;
        return Guid.Parse(userId.ToString());
    }
}
