using Microsoft.AspNetCore.Mvc;

namespace ProgPoe.Controllers
{
    public class DefaultUserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

