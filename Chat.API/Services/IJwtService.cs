namespace Chat.API.Services
{
    public interface IJwtService
    {
        string GenerateToken(Guid userId, string username, string fullName, bool isDepartmentAdmin, Guid departmentId);
    }
}
