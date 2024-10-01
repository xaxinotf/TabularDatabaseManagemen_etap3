using Microsoft.AspNetCore.Mvc;
using TabularDatabaseManagement.Services;

namespace TabularDatabaseManagement.Controllers
{
    public class HomeController : Controller
    {
        private readonly DatabaseService _dbService;

        public HomeController(DatabaseService dbService)
        {
            _dbService = dbService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult SaveDatabase()
        {
            try
            {
                _dbService.SaveToFile("database.json");
                return Content("База даних успішно збережена.");
            }
            catch (Exception ex)
            {
                return Content($"Помилка збереження бази даних: {ex.Message}");
            }
        }

        public IActionResult LoadDatabase()
        {
            try
            {
                _dbService.LoadFromFile("database.json");
                return Content("База даних успішно завантажена.");
            }
            catch (Exception ex)
            {
                return Content($"Помилка завантаження бази даних: {ex.Message}");
            }
        }
    }
}