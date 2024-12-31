using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System.Collections.Generic;
namespace CrudUsingDapperAndAjax.Models;

public class EmployeeController : Controller
{
    private readonly string _connectionString;

    public EmployeeController(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("dbcs");
    }

    // GET: Fetch all employees
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            var employees = await connection.QueryAsync<Employee>("SELECT * FROM Employees");
            ViewBag.list = employees;
            return View(employees);
        }
    }

    // POST: Add new employee
    [HttpPost]
    public async Task<IActionResult> Add(Employee employee)
    {
     
        
            var sql = "INSERT INTO Employees (Name, Position, Office, Age, Salary) VALUES (@Name, @Position, @Office, @Age, @Salary)";
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.ExecuteAsync(sql, employee);
            }
            return Json(new { success = true });
        
       
    }

    // GET: Get employee by id
    [HttpGet]
    public async Task<IActionResult> GetEmployee(int id)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            var employee = await connection.QueryFirstOrDefaultAsync<Employee>("SELECT * FROM Employees WHERE Id = @Id", new { Id = id });
            return Json(employee);
        }
    }

    // POST: Update employee
    [HttpPost]
    public async Task<IActionResult> Update(Employee employee)
    {
        if (ModelState.IsValid)
        {
            var sql = "UPDATE Employees SET Name = @Name, Position = @Position, Office = @Office, Age = @Age, Salary = @Salary WHERE Id = @Id";
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.ExecuteAsync(sql, employee);
            }
            return Json(new { success = true });
        }
        return Json(new { success = false });
    }

    // POST: Delete employee
    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var sql = "DELETE FROM Employees WHERE Id = @Id";
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.ExecuteAsync(sql, new { Id = id });
        }
        return Json(new { success = true });
    }
}
