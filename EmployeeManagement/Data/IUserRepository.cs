using EmployeeManagement.Models;

namespace EmployeeManagement.Data
{
    public interface IUserRepository
    {
        Task<User?> GetUserByUserNameAsync(string username);
        Task<User?> GetUserByEmailAsync(string email);
        Task<int> AddUserAsync(User user);
    }
}
