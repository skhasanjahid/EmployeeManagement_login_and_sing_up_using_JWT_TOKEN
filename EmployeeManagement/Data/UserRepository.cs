using Dapper;
using System.Data;
using EmployeeManagement.Models;

namespace EmployeeManagement.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly IDbConnection _db;
        public UserRepository(IDbConnection db) => _db = db;

        public async Task<User?> GetUserByUserNameAsync(string username)
        {
            var sql = "SELECT * FROM Users WHERE UserName = @UserName";
            return await _db.QueryFirstOrDefaultAsync<User>(sql, new { UserName = username });
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            var sql = "SELECT * FROM Users WHERE Email = @Email";
            return await _db.QueryFirstOrDefaultAsync<User>(sql, new { Email = email });
        }

        public async Task<int> AddUserAsync(User user)
        {
            var sql = @"INSERT INTO Users (UserName, Email, PasswordHash, Role)
                        VALUES (@UserName, @Email, @PasswordHash, @Role)";
            return await _db.ExecuteAsync(sql, user);
        }
    }
}
