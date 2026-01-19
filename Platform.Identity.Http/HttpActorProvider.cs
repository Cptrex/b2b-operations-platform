using Platform.Identity.Abstractions;
using Platform.Identity.Abstractions.Contracts;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Platform.Identity.Http;

public class HttpActorProvider : IActorProvider
{
    private readonly IHttpContextAccessor _accessor;

    public HttpActorProvider(IHttpContextAccessor accessor)
    {
        _accessor = accessor;
    }

    public ActorContext Current
    {
        get
        {
            var user = _accessor.HttpContext?.User;

            if (user?.Identity?.IsAuthenticated != true)
            {
                return new ActorContext(null, null, null, ActorType.Unknown);
            }

            var sub = user.FindFirst("sub") ?? user.FindFirst(ClaimTypes.NameIdentifier);

            if (sub is null)
            {
                return new ActorContext(null, null, null, ActorType.Unknown);
            }

            var iss = user.FindFirst("iss");

            if (iss is null)
            {
                return new ActorContext(null, null, null, ActorType.Unknown);
            }

            var type = iss.Value switch
            {
                "auth.business" => ActorType.User,
                "auth.service" => ActorType.Service,
                _ => ActorType.User
            };

            Guid? userId = Guid.TryParse(sub.Value, out var id) ? id : null;

            return new ActorContext(userId.ToString(), iss.Value, sub.Value, type);
        }
    }

    public ActorContext? GetCurrent()
    {
        return Current;
    }
}