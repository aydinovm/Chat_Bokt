using System.Security.Claims;

namespace Chat.API.Extensions
{
    public static class IdentityExtensions
    {
        public static Guid GetUserId(this ClaimsPrincipal user)
        {
            var claim = user.Claims.FirstOrDefault(x => x.Type == "UserId");
            if (claim != null && Guid.TryParse(claim.Value, out var userId))
            {
                return userId;
            }
            return Guid.Empty;
        }

        public static string GetUsername(this ClaimsPrincipal user)
        {
            return user.FindFirst("Username")?.Value
                   ?? user.FindFirst(ClaimTypes.Name)?.Value
                   ?? string.Empty;
        }

        public static Guid GetDepartmentId(this ClaimsPrincipal user)
        {
            var claim = user.Claims.FirstOrDefault(x => x.Type == "DepartmentId");
            if (claim != null && Guid.TryParse(claim.Value, out var deptId))
            {
                return deptId;
            }
            return Guid.Empty;
        }

        public static bool IsDepartmentAdmin(this ClaimsPrincipal user)
        {
            var claim = user.Claims.FirstOrDefault(x => x.Type == "IsDepartmentAdmin");
            return claim != null && bool.TryParse(claim.Value, out var isAdmin) && isAdmin;
        }

        public static bool IsAuthenticated(this ClaimsPrincipal user)
        {
            return user?.Identity?.IsAuthenticated ?? false;
        }
    }
}
