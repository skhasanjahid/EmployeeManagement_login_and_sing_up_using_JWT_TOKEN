using Dapper;
using System.Data;
using EmployeeManagement.Models;

namespace EmployeeManagement.Data
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly IDbConnection _db;
        public EmployeeRepository(IDbConnection db) => _db = db;

        public async Task<IEnumerable<Employee>> GetAllAsync()
            => await _db.QueryAsync<Employee>("SELECT * FROM Employees");

        public async Task<Employee?> GetByIdAsync(int id)
            => await _db.QueryFirstOrDefaultAsync<Employee>("SELECT * FROM Employees WHERE Id=@Id", new { Id = id });

        public async Task<int> AddAsync(Employee emp)
        {
            var sql = "INSERT INTO Employees (Name, Position, Salary) VALUES (@Name, @Position, @Salary)";
            return await _db.ExecuteAsync(sql, emp);
        }

        public async Task<int> UpdateAsync(Employee emp)
        {
            var sql = "UPDATE Employees SET Name=@Name, Position=@Position, Salary=@Salary WHERE Id=@Id";
            return await _db.ExecuteAsync(sql, emp);
        }

        public async Task<int> DeleteAsync(int id)
        {
            var sql = "DELETE FROM Employees WHERE Id=@Id";
            return await _db.ExecuteAsync(sql, new { Id = id });
        }
    }
}
