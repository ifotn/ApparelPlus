using Microsoft.AspNetCore.Mvc;

namespace ApparelPlus.Controllers
{
    public class StoreController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
