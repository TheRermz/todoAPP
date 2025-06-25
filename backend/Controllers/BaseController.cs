using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace todoApp.Controllers;

public abstract class BaseController : ControllerBase
{
    protected int? GetUserIdFromClaims()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null) return null;

        if (!int.TryParse(userIdClaim.Value, out var userId)) return null;

        return userId;
    }
}
