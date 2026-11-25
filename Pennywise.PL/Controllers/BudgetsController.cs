using Microsoft.AspNetCore.Mvc;

namespace Pennywise.PL.Controllers
{
    public class BudgetsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
