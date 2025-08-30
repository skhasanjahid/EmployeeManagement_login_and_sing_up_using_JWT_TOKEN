using EmployeeManagement.Models;

namespace EmployeeManagement.Services
{
    public interface IJwtService
    {
        string GenerateToken(User user);
    }
}
