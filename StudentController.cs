using CrudUsingDapperAndAjax.Models;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace CrudUsingDapperAndAjax.Controllers
{
    public class StudentController : Controller
    {
        private readonly string _connectionString;

        public StudentController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("dbcs");
        }

        // GET: AddStudent
        public async Task<IActionResult> AddStudent(int page = 1, int pageSize = 5)
        {
            using (var connection = new SqlConnection(_connectionString))
            {

               
                var totalRecords = await connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM Employees");
                var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

                // Fetch only the data for the current page
                var skip = (page - 1) * pageSize;
                var employees = await connection.QueryAsync<Employee>(
                    "SELECT * FROM Employees ORDER BY Id OFFSET @Skip ROWS FETCH NEXT @PageSize ROWS ONLY",
                    new { Skip = skip, PageSize = pageSize }
                );

                ViewBag.CurrentPage = page;
                ViewBag.TotalPages = totalPages;
                ViewBag.PageSize = pageSize;
                ViewBag.HasPrevious = page > 1;
                ViewBag.HasNext = page < totalPages;

                ViewBag.List = employees;
                return View();
            }
        }

        // POST: AddStudent
        [HttpPost]
        [ValidateAntiForgeryToken] // Helps prevent Cross-Site Request Forgery attacks
        public async Task<IActionResult> AddStudent(Employee emp)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (var connection = new SqlConnection(_connectionString))
                    {
                        var sql = "INSERT INTO Employees (Name, Position, Office, Age, Salary) VALUES (@Name, @Position, @Office, @Age, @Salary)";
                        await connection.ExecuteAsync(sql, emp);
                    }

                    // Redirect to the AddStudent action to avoid re-submission on refresh
                    return RedirectToAction("AddStudent");
                }
                catch (Exception ex)
                {
                    // Log error and show an error message to the user
                    ModelState.AddModelError(string.Empty, "An error occurred while saving the data: " + ex.Message);
                }
            }

            // In case of an error, we return the same view with validation messages
            return View(emp);
        }
    }

}
