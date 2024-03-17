using Microsoft.AspNetCore.Mvc;

namespace BulkyBookWeb.Controllers
{
    public class OwnershipController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
