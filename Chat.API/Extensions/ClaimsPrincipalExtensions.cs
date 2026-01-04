using System.Security.Claims;

namespace Chat.API.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static Guid GetUserId(this ClaimsPrincipal principal)
        {
            var userIdClaim = principal.FindFirst("UserId")?.Value
                ?? principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
        }

        public static string GetUsername(this ClaimsPrincipal principal)
        {
            return principal.FindFirst("Username")?.Value
                ?? principal.FindFirst(ClaimTypes.Name)?.Value
                ?? string.Empty;
        }

        public static Guid GetDepartmentId(this ClaimsPrincipal principal)
        {
            var deptIdClaim = principal.FindFirst("DepartmentId")?.Value;
            return Guid.TryParse(deptIdClaim, out var deptId) ? deptId : Guid.Empty;
        }

        public static bool IsDepartmentAdmin(this ClaimsPrincipal principal)
        {
            var adminClaim = principal.FindFirst("IsDepartmentAdmin")?.Value;
            return bool.TryParse(adminClaim, out var isAdmin) && isAdmin;
        }

        public static bool IsAuthenticated(this ClaimsPrincipal principal)
        {
            return principal?.Identity?.IsAuthenticated ?? false;
        }
    }
}
