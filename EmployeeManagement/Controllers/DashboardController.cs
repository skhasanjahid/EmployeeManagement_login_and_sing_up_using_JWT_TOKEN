using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View("Dashboard");
        }
    }
}
